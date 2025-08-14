using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TicketManagementService.Models;

public class Ticket
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    
    [Required]
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    
    [Required]
    public string Category { get; set; } = string.Empty;
    
    public string? SubCategory { get; set; }
    
    [Required]
    public string CustomerEmail { get; set; } = string.Empty;
    
    public string? CustomerName { get; set; }
    
    public string? CustomerPhone { get; set; }
    
    public Guid? AssignedAgentId { get; set; }
    
    public string? AssignedAgentName { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? ResolvedAt { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public int EstimatedHours { get; set; }
    
    public int ActualHours { get; set; }
    
    public string? Tags { get; set; }
    
    public string? Attachments { get; set; }
    
    public string? InternalNotes { get; set; }
    
    public string? Resolution { get; set; }
    
    public decimal? CustomerSatisfactionScore { get; set; }
    
    public string? CustomerFeedback { get; set; }
    
    // AI-generated fields
    public string? AICategory { get; set; }
    
    public double? AIConfidence { get; set; }
    
    public string? AIPriority { get; set; }
    
    public double? PriorityConfidence { get; set; }
    
    public DateTime? LastAIAnalysis { get; set; }
}

public enum TicketStatus
{
    Open,
    InProgress,
    WaitingForCustomer,
    WaitingForThirdParty,
    Resolved,
    Closed,
    Cancelled
}

public enum TicketPriority
{
    Low,
    Medium,
    High,
    Critical,
    Emergency
}
