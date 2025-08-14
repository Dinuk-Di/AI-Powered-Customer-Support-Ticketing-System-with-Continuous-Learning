namespace AICategorizationService.Models;

public class TicketAnalysisRequest
{
    public Guid TicketId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Tags { get; set; }
    public string? Attachments { get; set; }
}

public class TicketAnalysisResponse
{
    public Guid TicketId { get; set; }
    public string PredictedCategory { get; set; } = string.Empty;
    public double CategoryConfidence { get; set; }
    public string PredictedSubCategory { get; set; } = string.Empty;
    public double SubCategoryConfidence { get; set; }
    public List<string> SuggestedTags { get; set; } = new();
    public double OverallConfidence { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
    public DateTime AnalysisTimestamp { get; set; }
    public Dictionary<string, double> CategoryProbabilities { get; set; } = new();
    public Dictionary<string, double> SubCategoryProbabilities { get; set; } = new();
}

public class BatchAnalysisRequest
{
    public List<TicketAnalysisRequest> Tickets { get; set; } = new();
    public bool PrioritizeByUrgency { get; set; } = true;
    public int MaxBatchSize { get; set; } = 100;
}

public class BatchAnalysisResponse
{
    public List<TicketAnalysisResponse> Results { get; set; } = new();
    public int TotalProcessed { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime BatchStartTime { get; set; }
    public DateTime BatchEndTime { get; set; }
    public TimeSpan ProcessingTime { get; set; }
}
