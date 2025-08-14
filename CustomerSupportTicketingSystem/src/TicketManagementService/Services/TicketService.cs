using TicketManagementService.Models;
using TicketManagementService.Models.DTOs;

namespace TicketManagementService.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;

    public TicketService(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
    {
        return await _ticketRepository.GetAllAsync();
    }

    public async Task<Ticket?> GetTicketByIdAsync(Guid id)
    {
        return await _ticketRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(TicketStatus status)
    {
        return await _ticketRepository.GetByStatusAsync(status);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByPriorityAsync(TicketPriority priority)
    {
        return await _ticketRepository.GetByPriorityAsync(priority);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByCategoryAsync(string category)
    {
        return await _ticketRepository.GetByCategoryAsync(category);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByCustomerEmailAsync(string email)
    {
        return await _ticketRepository.GetByCustomerEmailAsync(email);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByAssignedAgentAsync(Guid agentId)
    {
        return await _ticketRepository.GetByAssignedAgentAsync(agentId);
    }

    public async Task<IEnumerable<Ticket>> GetOverdueTicketsAsync()
    {
        return await _ticketRepository.GetOverdueTicketsAsync();
    }

    public async Task<IEnumerable<Ticket>> SearchTicketsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await _ticketRepository.GetAllAsync();

        return await _ticketRepository.SearchTicketsAsync(searchTerm);
    }

    public async Task<Ticket> CreateTicketAsync(CreateTicketDto createDto)
    {
        var ticket = new Ticket
        {
            Title = createDto.Title,
            Description = createDto.Description,
            CustomerEmail = createDto.CustomerEmail,
            CustomerName = createDto.CustomerName,
            CustomerPhone = createDto.CustomerPhone,
            Category = createDto.Category ?? "General",
            SubCategory = createDto.SubCategory,
            DueDate = createDto.DueDate,
            EstimatedHours = createDto.EstimatedHours,
            Tags = createDto.Tags,
            Attachments = createDto.Attachments
        };

        return await _ticketRepository.CreateAsync(ticket);
    }

    public async Task<Ticket> UpdateTicketAsync(Guid id, UpdateTicketDto updateDto)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null)
            throw new ArgumentException($"Ticket with ID {id} not found");

        // Update only the fields that are provided
        if (updateDto.Title != null)
            ticket.Title = updateDto.Title;
        
        if (updateDto.Description != null)
            ticket.Description = updateDto.Description;
        
        if (updateDto.Status.HasValue)
            ticket.Status = updateDto.Status.Value;
        
        if (updateDto.Priority.HasValue)
            ticket.Priority = updateDto.Priority.Value;
        
        if (updateDto.Category != null)
            ticket.Category = updateDto.Category;
        
        if (updateDto.SubCategory != null)
            ticket.SubCategory = updateDto.SubCategory;
        
        if (updateDto.AssignedAgentId.HasValue)
            ticket.AssignedAgentId = updateDto.AssignedAgentId.Value;
        
        if (updateDto.AssignedAgentName != null)
            ticket.AssignedAgentName = updateDto.AssignedAgentName;
        
        if (updateDto.DueDate.HasValue)
            ticket.DueDate = updateDto.DueDate.Value;
        
        if (updateDto.EstimatedHours.HasValue)
            ticket.EstimatedHours = updateDto.EstimatedHours.Value;
        
        if (updateDto.ActualHours.HasValue)
            ticket.ActualHours = updateDto.ActualHours.Value;
        
        if (updateDto.Tags != null)
            ticket.Tags = updateDto.Tags;
        
        if (updateDto.Attachments != null)
            ticket.Attachments = updateDto.Attachments;
        
        if (updateDto.InternalNotes != null)
            ticket.InternalNotes = updateDto.InternalNotes;
        
        if (updateDto.Resolution != null)
            ticket.Resolution = updateDto.Resolution;
        
        if (updateDto.CustomerSatisfactionScore.HasValue)
            ticket.CustomerSatisfactionScore = updateDto.CustomerSatisfactionScore.Value;
        
        if (updateDto.CustomerFeedback != null)
            ticket.CustomerFeedback = updateDto.CustomerFeedback;

        return await _ticketRepository.UpdateAsync(ticket);
    }

    public async Task<bool> DeleteTicketAsync(Guid id)
    {
        return await _ticketRepository.DeleteAsync(id);
    }

    public async Task<Ticket> AssignTicketToAgentAsync(Guid ticketId, Guid agentId, string agentName)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new ArgumentException($"Ticket with ID {ticketId} not found");

        ticket.AssignedAgentId = agentId;
        ticket.AssignedAgentName = agentName;
        ticket.Status = TicketStatus.InProgress;

        return await _ticketRepository.UpdateAsync(ticket);
    }

    public async Task<Ticket> UpdateTicketStatusAsync(Guid ticketId, TicketStatus status)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new ArgumentException($"Ticket with ID {ticketId} not found");

        ticket.Status = status;

        if (status == TicketStatus.Resolved && !ticket.ResolvedAt.HasValue)
        {
            ticket.ResolvedAt = DateTime.UtcNow;
        }

        return await _ticketRepository.UpdateAsync(ticket);
    }

    public async Task<Ticket> UpdateTicketPriorityAsync(Guid ticketId, TicketPriority priority)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new ArgumentException($"Ticket with ID {ticketId} not found");

        ticket.Priority = priority;
        return await _ticketRepository.UpdateAsync(ticket);
    }

    public async Task<Ticket> ResolveTicketAsync(Guid ticketId, string resolution)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new ArgumentException($"Ticket with ID {ticketId} not found");

        ticket.Status = TicketStatus.Resolved;
        ticket.Resolution = resolution;
        ticket.ResolvedAt = DateTime.UtcNow;

        return await _ticketRepository.UpdateAsync(ticket);
    }

    public async Task<Ticket> AddCustomerFeedbackAsync(Guid ticketId, decimal satisfactionScore, string feedback)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new ArgumentException($"Ticket with ID {ticketId} not found");

        ticket.CustomerSatisfactionScore = satisfactionScore;
        ticket.CustomerFeedback = feedback;

        return await _ticketRepository.UpdateAsync(ticket);
    }

    public async Task<Dictionary<TicketStatus, int>> GetTicketStatusCountsAsync()
    {
        var statuses = Enum.GetValues<TicketStatus>();
        var counts = new Dictionary<TicketStatus, int>();

        foreach (var status in statuses)
        {
            counts[status] = await _ticketRepository.GetCountByStatusAsync(status);
        }

        return counts;
    }

    public async Task<IEnumerable<Ticket>> GetTicketsForAIAnalysisAsync()
    {
        return await _ticketRepository.GetTicketsForAIAnalysisAsync();
    }

    public async Task UpdateAIAnalysisAsync(Guid ticketId, string aiCategory, double confidence, string aiPriority, double priorityConfidence)
    {
        await _ticketRepository.UpdateAIAnalysisAsync(ticketId, aiCategory, confidence, aiPriority, priorityConfidence);
    }
}
