using AICategorizationService.Models;

namespace AICategorizationService.Services;

public interface IMLModelService
{
    Task<TicketAnalysisResponse> AnalyzeTicketAsync(TicketAnalysisRequest request);
    Task<BatchAnalysisResponse> AnalyzeBatchAsync(BatchAnalysisRequest request);
    Task<bool> TrainModelAsync(string trainingDataPath);
    Task<bool> UpdateModelAsync(string newModelPath);
    Task<ModelInfo> GetModelInfoAsync();
    Task<double> EvaluateModelAsync(string testDataPath);
    Task<List<string>> GetAvailableCategoriesAsync();
    Task<List<string>> GetAvailableSubCategoriesAsync(string category);
    Task<bool> IsModelReadyAsync();
    Task<Dictionary<string, double>> GetCategoryProbabilitiesAsync(string text);
    Task<Dictionary<string, double>> GetSubCategoryProbabilitiesAsync(string text, string category);
}

public class ModelInfo
{
    public string ModelVersion { get; set; } = string.Empty;
    public DateTime LastTrained { get; set; }
    public double Accuracy { get; set; }
    public int TrainingSampleCount { get; set; }
    public string ModelPath { get; set; } = string.Empty;
    public TimeSpan TrainingTime { get; set; }
    public Dictionary<string, double> CategoryAccuracies { get; set; } = new();
    public Dictionary<string, double> SubCategoryAccuracies { get; set; } = new();
}
