using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using AICategorizationService.Models;

namespace AICategorizationService.Services;

public class MLModelService : IMLModelService
{
    private readonly MLContext _mlContext;
    private ITransformer? _categoryModel;
    private ITransformer? _subCategoryModel;
    private PredictionEngine<TicketData, CategoryPrediction>? _categoryPredictor;
    private PredictionEngine<TicketData, SubCategoryPrediction>? _subCategoryPredictor;
    private readonly string _modelDirectory;
    private readonly ILogger<MLModelService> _logger;

    public MLModelService(ILogger<MLModelService> logger)
    {
        _mlContext = new MLContext(seed: 42);
        _modelDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models");
        _logger = logger;
        
        // Ensure model directory exists
        if (!Directory.Exists(_modelDirectory))
        {
            Directory.CreateDirectory(_modelDirectory);
        }
        
        LoadModels();
    }

    public async Task<TicketAnalysisResponse> AnalyzeTicketAsync(TicketAnalysisRequest request)
    {
        try
        {
            if (!await IsModelReadyAsync())
            {
                throw new InvalidOperationException("ML models are not ready for analysis");
            }

            var ticketData = new TicketData
            {
                Title = request.Title,
                Description = request.Description,
                CustomerEmail = request.CustomerEmail ?? string.Empty,
                Category = request.Category ?? string.Empty,
                SubCategory = request.SubCategory ?? string.Empty,
                Tags = request.Tags ?? string.Empty,
                Attachments = request.Attachments ?? string.Empty
            };

            var categoryPrediction = _categoryPredictor!.Predict(ticketData);
            var subCategoryPrediction = _subCategoryPredictor!.Predict(ticketData);

            var categoryProbabilities = await GetCategoryProbabilitiesAsync(request.Title + " " + request.Description);
            var subCategoryProbabilities = await GetSubCategoryProbabilitiesAsync(request.Title + " " + request.Description, categoryPrediction.Category);

            var response = new TicketAnalysisResponse
            {
                TicketId = request.TicketId,
                PredictedCategory = categoryPrediction.Category,
                CategoryConfidence = categoryPrediction.Score.Max(),
                PredictedSubCategory = subCategoryPrediction.SubCategory,
                SubCategoryConfidence = subCategoryPrediction.Score.Max(),
                SuggestedTags = GenerateSuggestedTags(request.Title, request.Description, categoryPrediction.Category),
                OverallConfidence = (categoryPrediction.Score.Max() + subCategoryPrediction.Score.Max()) / 2,
                ModelVersion = "1.0.0",
                AnalysisTimestamp = DateTime.UtcNow,
                CategoryProbabilities = categoryProbabilities,
                SubCategoryProbabilities = subCategoryProbabilities
            };

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing ticket {TicketId}", request.TicketId);
            throw;
        }
    }

    public async Task<BatchAnalysisResponse> AnalyzeBatchAsync(BatchAnalysisRequest request)
    {
        var response = new BatchAnalysisResponse
        {
            BatchStartTime = DateTime.UtcNow
        };

        try
        {
            var tickets = request.Tickets.Take(request.MaxBatchSize).ToList();
            response.TotalProcessed = tickets.Count;

            foreach (var ticket in tickets)
            {
                try
                {
                    var analysis = await AnalyzeTicketAsync(ticket);
                    response.Results.Add(analysis);
                    response.SuccessCount++;
                }
                catch (Exception ex)
                {
                    response.FailureCount++;
                    response.Errors.Add($"Ticket {ticket.TicketId}: {ex.Message}");
                    _logger.LogError(ex, "Error analyzing ticket {TicketId} in batch", ticket.TicketId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing batch analysis");
            response.Errors.Add($"Batch processing error: {ex.Message}");
        }
        finally
        {
            response.BatchEndTime = DateTime.UtcNow;
            response.ProcessingTime = response.BatchEndTime - response.BatchStartTime;
        }

        return response;
    }

    public async Task<bool> TrainModelAsync(string trainingDataPath)
    {
        try
        {
            if (!File.Exists(trainingDataPath))
            {
                _logger.LogError("Training data file not found: {Path}", trainingDataPath);
                return false;
            }

            // Load training data
            var trainingData = _mlContext.Data.LoadFromTextFile<TicketData>(trainingDataPath, separatorChar: ',');

            // Create and train category model
            var categoryPipeline = CreateCategoryPipeline();
            _categoryModel = categoryPipeline.Fit(trainingData);
            _categoryPredictor = _mlContext.Model.CreatePredictionEngine<TicketData, CategoryPrediction>(_categoryModel);

            // Create and train sub-category model
            var subCategoryPipeline = CreateSubCategoryPipeline();
            _subCategoryModel = subCategoryPipeline.Fit(trainingData);
            _subCategoryPredictor = _mlContext.Model.CreatePredictionEngine<TicketData, SubCategoryPrediction>(_subCategoryModel);

            // Save models
            var categoryModelPath = Path.Combine(_modelDirectory, "category_model.zip");
            var subCategoryModelPath = Path.Combine(_modelDirectory, "subcategory_model.zip");

            _mlContext.Model.Save(_categoryModel, trainingData.Schema, categoryModelPath);
            _mlContext.Model.Save(_subCategoryModel, trainingData.Schema, subCategoryModelPath);

            _logger.LogInformation("Models trained and saved successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error training models");
            return false;
        }
    }

    public async Task<bool> UpdateModelAsync(string newModelPath)
    {
        try
        {
            if (!File.Exists(newModelPath))
            {
                _logger.LogError("New model file not found: {Path}", newModelPath);
                return false;
            }

            // Load new model
            var newModel = _mlContext.Model.Load(newModelPath, out var schema);
            
            // Update the appropriate model based on file naming
            if (newModelPath.Contains("category"))
            {
                _categoryModel = newModel;
                _categoryPredictor = _mlContext.Model.CreatePredictionEngine<TicketData, CategoryPrediction>(_categoryModel);
            }
            else if (newModelPath.Contains("subcategory"))
            {
                _subCategoryModel = newModel;
                _subCategoryPredictor = _mlContext.Model.CreatePredictionEngine<TicketData, SubCategoryPrediction>(_subCategoryModel);
            }

            _logger.LogInformation("Model updated successfully: {Path}", newModelPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating model");
            return false;
        }
    }

    public async Task<ModelInfo> GetModelInfoAsync()
    {
        var info = new ModelInfo
        {
            ModelVersion = "1.0.0",
            LastTrained = DateTime.UtcNow.AddDays(-7), // This would come from actual model metadata
            Accuracy = 0.85, // This would come from actual evaluation
            TrainingSampleCount = 10000, // This would come from actual training data
            ModelPath = _modelDirectory,
            TrainingTime = TimeSpan.FromMinutes(30) // This would come from actual training
        };

        // Populate category accuracies (this would come from actual evaluation)
        info.CategoryAccuracies["Technical"] = 0.88;
        info.CategoryAccuracies["Billing"] = 0.82;
        info.CategoryAccuracies["General"] = 0.79;

        return info;
    }

    public async Task<double> EvaluateModelAsync(string testDataPath)
    {
        try
        {
            if (!File.Exists(testDataPath))
            {
                _logger.LogError("Test data file not found: {Path}", testDataPath);
                return 0.0;
            }

            var testData = _mlContext.Data.LoadFromTextFile<TicketData>(testDataPath, separatorChar: ',');
            
            // This is a simplified evaluation - in practice, you'd want more comprehensive metrics
            var predictions = _categoryPredictor!.Predict(testData);
            
            // Calculate accuracy (this is simplified)
            return 0.85; // Placeholder accuracy
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating model");
            return 0.0;
        }
    }

    public async Task<List<string>> GetAvailableCategoriesAsync()
    {
        return new List<string>
        {
            "Technical",
            "Billing",
            "General",
            "Feature Request",
            "Bug Report",
            "Account",
            "Security"
        };
    }

    public async Task<List<string>> GetAvailableSubCategoriesAsync(string category)
    {
        return category.ToLower() switch
        {
            "technical" => new List<string> { "Software", "Hardware", "Network", "Database", "API" },
            "billing" => new List<string> { "Payment", "Refund", "Invoice", "Subscription", "Pricing" },
            "general" => new List<string> { "Information", "Question", "Feedback", "Other" },
            "feature request" => new List<string> { "New Feature", "Enhancement", "Integration" },
            "bug report" => new List<string> { "Critical", "Major", "Minor", "Cosmetic" },
            "account" => new List<string> { "Login", "Registration", "Profile", "Permissions" },
            "security" => new List<string> { "Vulnerability", "Access", "Privacy", "Compliance" },
            _ => new List<string> { "General" }
        };
    }

    public async Task<bool> IsModelReadyAsync()
    {
        return _categoryPredictor != null && _subCategoryPredictor != null;
    }

    public async Task<Dictionary<string, double>> GetCategoryProbabilitiesAsync(string text)
    {
        if (_categoryPredictor == null)
            return new Dictionary<string, double>();

        var ticketData = new TicketData { Title = text, Description = text };
        var prediction = _categoryPredictor.Predict(ticketData);
        
        var categories = await GetAvailableCategoriesAsync();
        var probabilities = new Dictionary<string, double>();
        
        for (int i = 0; i < Math.Min(categories.Count, prediction.Score.Length); i++)
        {
            probabilities[categories[i]] = prediction.Score[i];
        }
        
        return probabilities;
    }

    public async Task<Dictionary<string, double>> GetSubCategoryProbabilitiesAsync(string text, string category)
    {
        if (_subCategoryPredictor == null)
            return new Dictionary<string, double>();

        var ticketData = new TicketData { Title = text, Description = text, Category = category };
        var prediction = _subCategoryPredictor.Predict(ticketData);
        
        var subCategories = await GetAvailableSubCategoriesAsync(category);
        var probabilities = new Dictionary<string, double>();
        
        for (int i = 0; i < Math.Min(subCategories.Count, prediction.Score.Length); i++)
        {
            probabilities[subCategories[i]] = prediction.Score[i];
        }
        
        return probabilities;
    }

    private void LoadModels()
    {
        try
        {
            var categoryModelPath = Path.Combine(_modelDirectory, "category_model.zip");
            var subCategoryModelPath = Path.Combine(_modelDirectory, "subcategory_model.zip");

            if (File.Exists(categoryModelPath))
            {
                _categoryModel = _mlContext.Model.Load(categoryModelPath, out var categorySchema);
                _categoryPredictor = _mlContext.Model.CreatePredictionEngine<TicketData, CategoryPrediction>(_categoryModel);
                _logger.LogInformation("Category model loaded successfully");
            }

            if (File.Exists(subCategoryModelPath))
            {
                _subCategoryModel = _mlContext.Model.Load(subCategoryModelPath, out var subCategorySchema);
                _subCategoryPredictor = _mlContext.Model.CreatePredictionEngine<TicketData, SubCategoryPrediction>(_subCategoryModel);
                _logger.LogInformation("Sub-category model loaded successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load existing models, will use default behavior");
        }
    }

    private EstimatorChain<MulticlassPredictionTransformer<Microsoft.ML.Trainers.SdcaMaximumEntropyMulticlassModelParameters>> CreateCategoryPipeline()
    {
        return _mlContext.Transforms.Text
            .FeaturizeText("Features", "Title", "Description")
            .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
            .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
    }

    private EstimatorChain<MulticlassPredictionTransformer<Microsoft.ML.Trainers.SdcaMaximumEntropyMulticlassModelParameters>> CreateSubCategoryPipeline()
    {
        return _mlContext.Transforms.Text
            .FeaturizeText("Features", "Title", "Description")
            .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
            .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
    }

    private List<string> GenerateSuggestedTags(string title, string description, string category)
    {
        var tags = new List<string>();
        var text = (title + " " + description).ToLower();

        // Add category-based tags
        tags.Add(category.ToLower());

        // Add common tags based on content
        if (text.Contains("urgent") || text.Contains("critical") || text.Contains("emergency"))
            tags.Add("urgent");

        if (text.Contains("bug") || text.Contains("error") || text.Contains("crash"))
            tags.Add("bug");

        if (text.Contains("feature") || text.Contains("request") || text.Contains("enhancement"))
            tags.Add("feature-request");

        if (text.Contains("billing") || text.Contains("payment") || text.Contains("invoice"))
            tags.Add("billing");

        return tags.Distinct().Take(5).ToList();
    }
}

// ML.NET data models
public class TicketData
{
    [LoadColumn(0)]
    public string Title { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string Description { get; set; } = string.Empty;

    [LoadColumn(2)]
    public string CustomerEmail { get; set; } = string.Empty;

    [LoadColumn(3)]
    public string Category { get; set; } = string.Empty;

    [LoadColumn(4)]
    public string SubCategory { get; set; } = string.Empty;

    [LoadColumn(5)]
    public string Tags { get; set; } = string.Empty;

    [LoadColumn(6)]
    public string Attachments { get; set; } = string.Empty;
}

public class CategoryPrediction
{
    [ColumnName("PredictedLabel")]
    public string Category { get; set; } = string.Empty;

    public float[] Score { get; set; } = Array.Empty<float>();
}

public class SubCategoryPrediction
{
    [ColumnName("PredictedLabel")]
    public string SubCategory { get; set; } = string.Empty;

    public float[] Score { get; set; } = Array.Empty<float>();
}
