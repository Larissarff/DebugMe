using DebugMeBackend.Entities;
using DebugMeBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DebugMeBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventLogController : ControllerBase
{
    private readonly EventLogService _eventLogService;

    public EventLogController(EventLogService eventLogService)
    {
        _eventLogService = eventLogService;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<EventLog>>> GetAll()
    {
        IEnumerable<EventLog> eventLogs = await _eventLogService.GetAllAsync();
        return Ok(eventLogs);
    }

    [HttpGet("id/{id:guid}")]
    public async Task<ActionResult<EventLog>> GetById(Guid id)
    {
        EventLog? eventLog = await _eventLogService.GetByIdAsync(id);
        return Ok(eventLog);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<EventLog>>> GetByUserId(Guid userId)
    {
        IEnumerable<EventLog> eventLogs = await _eventLogService.GetByUserIdAsync(userId);
        return Ok(eventLogs);
    }

    [HttpPost("create")]
    public async Task<ActionResult<EventLog>> Create([FromBody] EventLog eventLog)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            EventLog createdEventLog = await _eventLogService.CreateAsync(eventLog);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdEventLog.Id },
                createdEventLog
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update/{id:guid}")]
    public async Task<ActionResult<EventLog>> Update(Guid id, [FromBody] EventLog eventLog)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            EventLog updatedEventLog = await _eventLogService.UpdateAsync(id, eventLog);
            return Ok(updatedEventLog);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _eventLogService.DeleteAsync(id);
        return NoContent();
    }
}
