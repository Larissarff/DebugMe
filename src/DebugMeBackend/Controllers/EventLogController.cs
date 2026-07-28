using DebugMeBackend.DTOs.EventLog;
using DebugMeBackend.Entities;
using DebugMeBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DebugMeBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventLogController : ControllerBase
{
    private readonly EventLogService _eventLogService;

    public EventLogController(EventLogService eventLogService)
    {
        _eventLogService = eventLogService;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<EventLogResponseDto>>> GetAll()
    {
        IEnumerable<EventLog> eventLogs = await _eventLogService.GetAllAsync();
        IEnumerable<EventLogResponseDto> dtos = eventLogs.Select(MapToDto);
        return Ok(dtos);
    }

    [HttpGet("id/{id:guid}")]
    public async Task<ActionResult<EventLogResponseDto>> GetById(Guid id)
    {
        EventLog? eventLog = await _eventLogService.GetByIdAsync(id);
        if (eventLog is null)
            return NotFound(new { message = "Registro não encontrado." });
        return Ok(MapToDto(eventLog));
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<EventLogResponseDto>>> GetByUserId(Guid userId)
    {
        IEnumerable<EventLog> eventLogs = await _eventLogService.GetByUserIdAsync(userId);
        IEnumerable<EventLogResponseDto> dtos = eventLogs.Select(MapToDto);
        return Ok(dtos);
    }

    [HttpPost("create")]
    public async Task<ActionResult<EventLogResponseDto>> Create([FromBody] CreateEventLogDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            EventLog eventLog = new EventLog
            {
                UserId = dto.UserId,
                EmotionId = dto.EmotionId,
                Description = dto.Description ?? string.Empty,
                Intensity = dto.Intensity,
                EventDate = dto.EventDate
            };

            EventLog createdEventLog = await _eventLogService.CreateAsync(eventLog);
            EventLogResponseDto responseDto = MapToDto(createdEventLog);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdEventLog.Id },
                responseDto
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update/{id:guid}")]
    public async Task<ActionResult<EventLogResponseDto>> Update(Guid id, [FromBody] EventLog eventLog)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            EventLog updatedEventLog = await _eventLogService.UpdateAsync(id, eventLog);
            return Ok(MapToDto(updatedEventLog));
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

    private static EventLogResponseDto MapToDto(EventLog eventLog)
    {
        return new EventLogResponseDto
        {
            Id = eventLog.Id,
            UserId = eventLog.UserId,
            EmotionId = eventLog.EmotionId,
            Description = eventLog.Description,
            Intensity = eventLog.Intensity,
            EventDate = eventLog.EventDate,
            CreatedAt = eventLog.CreatedAt,
            UpdatedAt = eventLog.UpdatedAt,
            Emotion = eventLog.Emotion is not null
                ? new EmotionInfoDto
                {
                    Id = eventLog.Emotion.Id,
                    Name = eventLog.Emotion.Name,
                    Description = eventLog.Emotion.Description
                }
                : null,
            User = eventLog.User is not null
                ? new UserInfoDto
                {
                    Id = eventLog.User.Id,
                    Name = eventLog.User.Name,
                    Email = eventLog.User.Email
                }
                : null
        };
    }
}
