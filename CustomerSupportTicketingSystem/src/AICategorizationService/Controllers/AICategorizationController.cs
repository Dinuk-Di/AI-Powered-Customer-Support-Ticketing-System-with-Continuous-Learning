using Microsoft.AspNetCore.Mvc;
using AICategorizationService.Models;
using AICategorizationService.Services;

namespace AICategorizationService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AICategorizationController : ControllerBase
{
    private readonly IMLModelService _mlModelService;
    private readonly ILogger<AICategorizationController> _logger;

    public AICategorizationController(IMLModelService mlModelService, ILogger<AICategorizationController> logger)
    {
        _mlModelService = mlModelService;
        _logger = logger;
    }

    /// <summary>
    /// Analyze a single ticket for AI categorization
    /// </summary>
    [HttpPost("analyze")]
    [ProducesResponseType(typeof(TicketAnalysisResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TicketAnalysisResponse>> AnalyzeTicket([FromBody] TicketAnalysisRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Title) && string.IsNullOrWhiteSpace(request.Description))
                return BadRequest("At least one of Title or Description must be provided");

            var response = await _mlModelService.AnalyzeTicketAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "ML model not ready for ticket {TicketId}", request.TicketId);
            return StatusCode(503, "AI service is not ready. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing ticket {TicketId}", request.TicketId);
            return StatusCode(500, "An error occurred while analyzing the ticket");
        }
    }

    /// <summary>
    /// Analyze multiple tickets in batch for AI categorization
    /// </summary>
    [HttpPost("analyze/batch")]
    [ProducesResponseType(typeof(BatchAnalysisResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BatchAnalysisResponse>> AnalyzeBatch([FromBody] BatchAnalysisRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.Tickets == null || !request.Tickets.Any())
                return BadRequest("At least one ticket must be provided for batch analysis");

            if (request.Tickets.Count > request.MaxBatchSize)
                return BadRequest($"Batch size cannot exceed {request.MaxBatchSize} tickets");

            var response = await _mlModelService.AnalyzeBatchAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing batch analysis");
            return StatusCode(500, "An error occurred while processing batch analysis");
        }
    }

    /// <summary>
    /// Train the ML model with new data
    /// </summary>
    [HttpPost("train")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> TrainModel([FromBody] TrainModelRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.TrainingDataPath))
                return BadRequest("Training data path is required");

            if (!File.Exists(request.TrainingDataPath))
                return BadRequest("Training data file not found");

            var success = await _mlModelService.TrainModelAsync(request.TrainingDataPath);
            
            if (success)
            {
                _logger.LogInformation("Model training completed successfully");
                return Ok(new { message = "Model training completed successfully" });
            }
            else
            {
                return StatusCode(500, "Model training failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error training model");
            return StatusCode(500, "An error occurred while training the model");
        }
    }

    /// <summary>
    /// Update the ML model with a new version
    /// </summary>
    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateModel([FromBody] UpdateModelRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.NewModelPath))
                return BadRequest("New model path is required");

            if (!File.Exists(request.NewModelPath))
                return BadRequest("New model file not found");

            var success = await _mlModelService.UpdateModelAsync(request.NewModelPath);
            
            if (success)
            {
                _logger.LogInformation("Model updated successfully");
                return Ok(new { message = "Model updated successfully" });
            }
            else
            {
                return StatusCode(500, "Model update failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating model");
            return StatusCode(500, "An error occurred while updating the model");
        }
    }

    /// <summary>
    /// Get information about the current ML model
    /// </summary>
    [HttpGet("model/info")]
    [ProducesResponseType(typeof(ModelInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ModelInfo>> GetModelInfo()
    {
        try
        {
            var modelInfo = await _mlModelService.GetModelInfoAsync();
            return Ok(modelInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving model info");
            return StatusCode(500, "An error occurred while retrieving model information");
        }
    }

    /// <summary>
    /// Evaluate the current ML model performance
    /// </summary>
    [HttpPost("evaluate")]
    [ProducesResponseType(typeof(ModelEvaluationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ModelEvaluationResponse>> EvaluateModel([FromBody] EvaluateModelRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.TestDataPath))
                return BadRequest("Test data path is required");

            if (!File.Exists(request.TestDataPath))
                return BadRequest("Test data file not found");

            var accuracy = await _mlModelService.EvaluateModelAsync(request.TestDataPath);
            
            var response = new ModelEvaluationResponse
            {
                Accuracy = accuracy,
                EvaluationTimestamp = DateTime.UtcNow,
                TestDataPath = request.TestDataPath
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating model");
            return StatusCode(500, "An error occurred while evaluating the model");
        }
    }

    /// <summary>
    /// Get available categories for classification
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<string>>> GetAvailableCategories()
    {
        try
        {
            var categories = await _mlModelService.GetAvailableCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available categories");
            return StatusCode(500, "An error occurred while retrieving categories");
        }
    }

    /// <summary>
    /// Get available sub-categories for a specific category
    /// </summary>
    [HttpGet("categories/{category}/subcategories")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<string>>> GetAvailableSubCategories(string category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(category))
                return BadRequest("Category is required");

            var subCategories = await _mlModelService.GetAvailableSubCategoriesAsync(category);
            return Ok(subCategories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sub-categories for category {Category}", category);
            return StatusCode(500, "An error occurred while retrieving sub-categories");
        }
    }

    /// <summary>
    /// Check if the ML model is ready for analysis
    /// </summary>
    [HttpGet("ready")]
    [ProducesResponseType(typeof(ModelReadyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ModelReadyResponse>> IsModelReady()
    {
        try
        {
            var isReady = await _mlModelService.IsModelReadyAsync();
            
            var response = new ModelReadyResponse
            {
                IsReady = isReady,
                CheckTimestamp = DateTime.UtcNow
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking model readiness");
            return StatusCode(500, "An error occurred while checking model readiness");
        }
    }

    /// <summary>
    /// Get category probabilities for a given text
    /// </summary>
    [HttpPost("probabilities/categories")]
    [ProducesResponseType(typeof(Dictionary<string, double>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Dictionary<string, double>>> GetCategoryProbabilities([FromBody] TextAnalysisRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest("Text is required for analysis");

            var probabilities = await _mlModelService.GetCategoryProbabilitiesAsync(request.Text);
            return Ok(probabilities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category probabilities");
            return StatusCode(500, "An error occurred while analyzing text");
        }
    }

    /// <summary>
    /// Get sub-category probabilities for a given text and category
    /// </summary>
    [HttpPost("probabilities/subcategories")]
    [ProducesResponseType(typeof(Dictionary<string, double>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Dictionary<string, double>>> GetSubCategoryProbabilities([FromBody] SubCategoryAnalysisRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest("Text is required for analysis");

            if (string.IsNullOrWhiteSpace(request.Category))
                return BadRequest("Category is required for sub-category analysis");

            var probabilities = await _mlModelService.GetSubCategoryProbabilitiesAsync(request.Text, request.Category);
            return Ok(probabilities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sub-category probabilities");
            return StatusCode(500, "An error occurred while analyzing text");
        }
    }
}

// Additional DTOs for specific operations
public class TrainModelRequest
{
    public string TrainingDataPath { get; set; } = string.Empty;
}

public class UpdateModelRequest
{
    public string NewModelPath { get; set; } = string.Empty;
}

public class EvaluateModelRequest
{
    public string TestDataPath { get; set; } = string.Empty;
}

public class ModelEvaluationResponse
{
    public double Accuracy { get; set; }
    public DateTime EvaluationTimestamp { get; set; }
    public string TestDataPath { get; set; } = string.Empty;
}

public class ModelReadyResponse
{
    public bool IsReady { get; set; }
    public DateTime CheckTimestamp { get; set; }
}

public class TextAnalysisRequest
{
    public string Text { get; set; } = string.Empty;
}

public class SubCategoryAnalysisRequest
{
    public string Text { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
