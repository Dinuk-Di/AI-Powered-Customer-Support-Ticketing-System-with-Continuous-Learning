using TicketManagementService.Models;

namespace TicketManagementService.Services;

public interface ITicketRepository
{
    Task<IEnumerable<Ticket>> GetAllAsync();
    Task<Ticket?> GetByIdAsync(Guid id);
    Task<IEnumerable<Ticket>> GetByStatusAsync(TicketStatus status);
    Task<IEnumerable<Ticket>> GetByPriorityAsync(TicketPriority priority);
    Task<IEnumerable<Ticket>> GetByCategoryAsync(string category);
    Task<IEnumerable<Ticket>> GetByCustomerEmailAsync(string email);
    Task<IEnumerable<Ticket>> GetByAssignedAgentAsync(Guid agentId);
    Task<IEnumerable<Ticket>> GetOverdueTicketsAsync();
    Task<IEnumerable<Ticket>> SearchTicketsAsync(string searchTerm);
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket> UpdateAsync(Ticket ticket);
    Task<bool> DeleteAsync(Guid id);
    Task<int> GetCountByStatusAsync(TicketStatus status);
    Task<IEnumerable<Ticket>> GetTicketsForAIAnalysisAsync();
    Task UpdateAIAnalysisAsync(Guid ticketId, string aiCategory, double confidence, string aiPriority, double priorityConfidence);
}
