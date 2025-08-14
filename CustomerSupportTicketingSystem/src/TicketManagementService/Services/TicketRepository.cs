using Microsoft.EntityFrameworkCore;
using TicketManagementService.Data;
using TicketManagementService.Models;

namespace TicketManagementService.Services;

public class TicketRepository : ITicketRepository
{
    private readonly TicketDbContext _context;

    public TicketRepository(TicketDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ticket>> GetAllAsync()
    {
        return await _context.Tickets
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Ticket?> GetByIdAsync(Guid id)
    {
        return await _context.Tickets.FindAsync(id);
    }

    public async Task<IEnumerable<Ticket>> GetByStatusAsync(TicketStatus status)
    {
        return await _context.Tickets
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByPriorityAsync(TicketPriority priority)
    {
        return await _context.Tickets
            .Where(t => t.Priority == priority)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByCategoryAsync(string category)
    {
        return await _context.Tickets
            .Where(t => t.Category == category)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByCustomerEmailAsync(string email)
    {
        return await _context.Tickets
            .Where(t => t.CustomerEmail == email)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByAssignedAgentAsync(Guid agentId)
    {
        return await _context.Tickets
            .Where(t => t.AssignedAgentId == agentId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetOverdueTicketsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Tickets
            .Where(t => t.DueDate.HasValue && t.DueDate < now && t.Status != TicketStatus.Resolved && t.Status != TicketStatus.Closed)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> SearchTicketsAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _context.Tickets
            .Where(t => t.Title.ToLower().Contains(term) || 
                       t.Description.ToLower().Contains(term) ||
                       t.CustomerEmail.ToLower().Contains(term) ||
                       t.CustomerName!.ToLower().Contains(term) ||
                       t.Category.ToLower().Contains(term))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        ticket.Id = Guid.NewGuid();
        ticket.CreatedAt = DateTime.UtcNow;
        ticket.Status = TicketStatus.Open;
        ticket.Priority = TicketPriority.Medium;
        
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<Ticket> UpdateAsync(Ticket ticket)
    {
        ticket.UpdatedAt = DateTime.UtcNow;
        
        if (ticket.Status == TicketStatus.Resolved && !ticket.ResolvedAt.HasValue)
        {
            ticket.ResolvedAt = DateTime.UtcNow;
        }
        
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return false;

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetCountByStatusAsync(TicketStatus status)
    {
        return await _context.Tickets.CountAsync(t => t.Status == status);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsForAIAnalysisAsync()
    {
        return await _context.Tickets
            .Where(t => t.Status == TicketStatus.Open && 
                       (t.AICategory == null || t.LastAIAnalysis == null || 
                        t.LastAIAnalysis < DateTime.UtcNow.AddHours(-24)))
            .OrderBy(t => t.CreatedAt)
            .Take(100)
            .ToListAsync();
    }

    public async Task UpdateAIAnalysisAsync(Guid ticketId, string aiCategory, double confidence, string aiPriority, double priorityConfidence)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket != null)
        {
            ticket.AICategory = aiCategory;
            ticket.AIConfidence = confidence;
            ticket.AIPriority = aiPriority;
            ticket.PriorityConfidence = priorityConfidence;
            ticket.LastAIAnalysis = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
        }
    }
}
