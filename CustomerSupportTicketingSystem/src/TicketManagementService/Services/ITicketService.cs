using TicketManagementService.Models;
using TicketManagementService.Models.DTOs;

namespace TicketManagementService.Services;

public interface ITicketService
{
    Task<IEnumerable<Ticket>> GetAllTicketsAsync();
    Task<Ticket?> GetTicketByIdAsync(Guid id);
    Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(TicketStatus status);
    Task<IEnumerable<Ticket>> GetTicketsByPriorityAsync(TicketPriority priority);
    Task<IEnumerable<Ticket>> GetTicketsByCategoryAsync(string category);
    Task<IEnumerable<Ticket>> GetTicketsByCustomerEmailAsync(string email);
    Task<IEnumerable<Ticket>> GetTicketsByAssignedAgentAsync(Guid agentId);
    Task<IEnumerable<Ticket>> GetOverdueTicketsAsync();
    Task<IEnumerable<Ticket>> SearchTicketsAsync(string searchTerm);
    Task<Ticket> CreateTicketAsync(CreateTicketDto createDto);
    Task<Ticket> UpdateTicketAsync(Guid id, UpdateTicketDto updateDto);
    Task<bool> DeleteTicketAsync(Guid id);
    Task<Ticket> AssignTicketToAgentAsync(Guid ticketId, Guid agentId, string agentName);
    Task<Ticket> UpdateTicketStatusAsync(Guid ticketId, TicketStatus status);
    Task<Ticket> UpdateTicketPriorityAsync(Guid ticketId, TicketPriority priority);
    Task<Ticket> ResolveTicketAsync(Guid ticketId, string resolution);
    Task<Ticket> AddCustomerFeedbackAsync(Guid ticketId, decimal satisfactionScore, string feedback);
    Task<Dictionary<TicketStatus, int>> GetTicketStatusCountsAsync();
    Task<IEnumerable<Ticket>> GetTicketsForAIAnalysisAsync();
    Task UpdateAIAnalysisAsync(Guid ticketId, string aiCategory, double confidence, string aiPriority, double priorityConfidence);
}
