using Microsoft.AspNetCore.Mvc;
using TicketManagementService.Models;
using TicketManagementService.Models.DTOs;
using TicketManagementService.Services;

namespace TicketManagementService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(ITicketService ticketService, ILogger<TicketsController> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }

    /// <summary>
    /// Get all tickets
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Ticket>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
    {
        try
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tickets");
            return StatusCode(500, "An error occurred while retrieving tickets");
        }
    }

    /// <summary>
    /// Get ticket by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Ticket>> GetTicket(Guid id)
    {
        try
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
                return NotFound($"Ticket with ID {id} not found");

            return Ok(ticket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ticket {TicketId}", id);
            return StatusCode(500, "An error occurred while retrieving the ticket");
        }
    }

    /// <summary>
    /// Create a new ticket
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Ticket), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Ticket>> CreateTicket([FromBody] CreateTicketDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ticket = await _ticketService.CreateTicketAsync(createDto);
            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ticket");
            return StatusCode(500, "An error occurred while creating the ticket");
        }
    }

    /// <summary>
    /// Update an existing ticket
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Ticket>> UpdateTicket(Guid id, [FromBody] UpdateTicketDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ticket = await _ticketService.UpdateTicketAsync(id, updateDto);
            return Ok(ticket);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ticket {TicketId}", id);
            return StatusCode(500, "An error occurred while updating the ticket");
        }
    }

    /// <summary>
    /// Delete a ticket
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteTicket(Guid id)
    {
        try
        {
            var deleted = await _ticketService.DeleteTicketAsync(id);
            if (!deleted)
                return NotFound($"Ticket with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting ticket {TicketId}", id);
            return StatusCode(500, "An error occurred while deleting the ticket");
        }
    }

    /// <summary>
    /// Get tickets by status
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<Ticket>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByStatus(TicketStatus status)
    {
        try
        {
            var tickets = await _ticketService.GetTicketsByStatusAsync(status);
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tickets by status {Status}", status);
            return StatusCode(500, "An error occurred while retrieving tickets");
        }
    }

    /// <summary>
    /// Get tickets by priority
    /// </summary>
    [HttpGet("priority/{priority}")]
    [ProducesResponseType(typeof(IEnumerable<Ticket>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByPriority(TicketPriority priority)
    {
        try
        {
            var tickets = await _ticketService.GetTicketsByPriorityAsync(priority);
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tickets by priority {Priority}", priority);
            return StatusCode(500, "An error occurred while retrieving tickets");
        }
    }

    /// <summary>
    /// Get tickets by category
    /// </summary>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IEnumerable<Ticket>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByCategory(string category)
    {
        try
        {
            var tickets = await _ticketService.GetTicketsByCategoryAsync(category);
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tickets by category {Category}", category);
            return StatusCode(500, "An error occurred while retrieving tickets");
        }
    }

    /// <summary>
    /// Search tickets
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<Ticket>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Ticket>>> SearchTickets([FromQuery] string q)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Search query is required");

            var tickets = await _ticketService.SearchTicketsAsync(q);
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tickets with query {Query}", q);
            return StatusCode(500, "An error occurred while searching tickets");
        }
    }

    /// <summary>
    /// Get overdue tickets
    /// </summary>
    [HttpGet("overdue")]
    [ProducesResponseType(typeof(IEnumerable<Ticket>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetOverdueTickets()
    {
        try
        {
            var tickets = await _ticketService.GetOverdueTicketsAsync();
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overdue tickets");
            return StatusCode(500, "An error occurred while retrieving overdue tickets");
        }
    }

    /// <summary>
    /// Assign ticket to agent
    /// </summary>
    [HttpPost("{id:guid}/assign")]
    [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Ticket>> AssignTicket(Guid id, [FromBody] AssignTicketDto assignDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ticket = await _ticketService.AssignTicketToAgentAsync(id, assignDto.AgentId, assignDto.AgentName);
            return Ok(ticket);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning ticket {TicketId} to agent {AgentId}", id, assignDto.AgentId);
            return StatusCode(500, "An error occurred while assigning the ticket");
        }
    }

    /// <summary>
    /// Update ticket status
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Ticket>> UpdateStatus(Guid id, [FromBody] UpdateStatusDto statusDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ticket = await _ticketService.UpdateTicketStatusAsync(id, statusDto.Status);
            return Ok(ticket);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for ticket {TicketId}", id);
            return StatusCode(500, "An error occurred while updating the ticket status");
        }
    }

    /// <summary>
    /// Resolve ticket
    /// </summary>
    [HttpPost("{id:guid}/resolve")]
    [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Ticket>> ResolveTicket(Guid id, [FromBody] ResolveTicketDto resolveDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ticket = await _ticketService.ResolveTicketAsync(id, resolveDto.Resolution);
            return Ok(ticket);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving ticket {TicketId}", id);
            return StatusCode(500, "An error occurred while resolving the ticket");
        }
    }

    /// <summary>
    /// Get ticket status counts
    /// </summary>
    [HttpGet("stats/status-counts")]
    [ProducesResponseType(typeof(Dictionary<TicketStatus, int>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Dictionary<TicketStatus, int>>> GetStatusCounts()
    {
        try
        {
            var counts = await _ticketService.GetTicketStatusCountsAsync();
            return Ok(counts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ticket status counts");
            return StatusCode(500, "An error occurred while retrieving status counts");
        }
    }

    /// <summary>
    /// Get tickets for AI analysis
    /// </summary>
    [HttpGet("ai/analysis")]
    [ProducesResponseType(typeof(IEnumerable<Ticket>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsForAIAnalysis()
    {
        try
        {
            var tickets = await _ticketService.GetTicketsForAIAnalysisAsync();
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tickets for AI analysis");
            return StatusCode(500, "An error occurred while retrieving tickets for AI analysis");
        }
    }

    /// <summary>
    /// Update AI analysis results
    /// </summary>
    [HttpPost("{id:guid}/ai/analysis")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAIAnalysis(Guid id, [FromBody] AIAnalysisDto aiAnalysisDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ticketService.UpdateAIAnalysisAsync(
                id, 
                aiAnalysisDto.Category, 
                aiAnalysisDto.Confidence, 
                aiAnalysisDto.Priority, 
                aiAnalysisDto.PriorityConfidence);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating AI analysis for ticket {TicketId}", id);
            return StatusCode(500, "An error occurred while updating AI analysis");
        }
    }
}

// Additional DTOs for specific operations
public class AssignTicketDto
{
    public Guid AgentId { get; set; }
    public string AgentName { get; set; } = string.Empty;
}

public class UpdateStatusDto
{
    public TicketStatus Status { get; set; }
}

public class ResolveTicketDto
{
    public string Resolution { get; set; } = string.Empty;
}

public class AIAnalysisDto
{
    public string Category { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Priority { get; set; } = string.Empty;
    public double PriorityConfidence { get; set; }
}
