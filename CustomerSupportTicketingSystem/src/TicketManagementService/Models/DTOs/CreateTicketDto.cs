using System.ComponentModel.DataAnnotations;

namespace TicketManagementService.Models.DTOs;

public class CreateTicketDto
{
    [Required]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters")]
    public string? CustomerName { get; set; }
    
    [Phone(ErrorMessage = "Invalid phone number")]
    public string? CustomerPhone { get; set; }
    
    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string? Category { get; set; }
    
    [StringLength(100, ErrorMessage = "Sub-category cannot exceed 100 characters")]
    public string? SubCategory { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public int EstimatedHours { get; set; }
    
    [StringLength(500, ErrorMessage = "Tags cannot exceed 500 characters")]
    public string? Tags { get; set; }
    
    [StringLength(500, ErrorMessage = "Attachments cannot exceed 500 characters")]
    public string? Attachments { get; set; }
}
