using System.ComponentModel.DataAnnotations;

namespace TicketManagementService.Models.DTOs;

public class UpdateTicketDto
{
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? Title { get; set; }
    
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }
    
    public TicketStatus? Status { get; set; }
    
    public TicketPriority? Priority { get; set; }
    
    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string? Category { get; set; }
    
    [StringLength(100, ErrorMessage = "Sub-category cannot exceed 100 characters")]
    public string? SubCategory { get; set; }
    
    public Guid? AssignedAgentId { get; set; }
    
    public string? AssignedAgentName { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public int? EstimatedHours { get; set; }
    
    public int? ActualHours { get; set; }
    
    [StringLength(500, ErrorMessage = "Tags cannot exceed 500 characters")]
    public string? Tags { get; set; }
    
    [StringLength(500, ErrorMessage = "Attachments cannot exceed 500 characters")]
    public string? Attachments { get; set; }
    
    [StringLength(1000, ErrorMessage = "Internal notes cannot exceed 1000 characters")]
    public string? InternalNotes { get; set; }
    
    [StringLength(2000, ErrorMessage = "Resolution cannot exceed 2000 characters")]
    public string? Resolution { get; set; }
    
    [Range(1, 5, ErrorMessage = "Customer satisfaction score must be between 1 and 5")]
    public decimal? CustomerSatisfactionScore { get; set; }
    
    [StringLength(1000, ErrorMessage = "Customer feedback cannot exceed 1000 characters")]
    public string? CustomerFeedback { get; set; }
}
