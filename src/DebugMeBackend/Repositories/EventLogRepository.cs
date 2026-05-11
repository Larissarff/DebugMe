using DebugMeBackend.Data;
using DebugMeBackend.Entities;
using DebugMeBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DebugMeBackend.Repositories;

public class EventLogRepository : IEventLogRepository
{
    private readonly AppDbContext _context;

    public EventLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EventLog?> GetByIdAsync(Guid id)
    {
        return await _context.EventLogs
            .Include(el => el.User)
            .Include(el => el.Emotion)
            .FirstOrDefaultAsync(el => el.Id == id);
    }

    public async Task<IEnumerable<EventLog>> GetAllAsync()
    {
        return await _context.EventLogs
            .Include(el => el.User)
            .Include(el => el.Emotion)
            .OrderByDescending(el => el.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventLog>> GetByUserIdAsync(Guid userId)
    {
        return await _context.EventLogs
            .Include(el => el.Emotion)
            .Where(el => el.UserId == userId)
            .OrderByDescending(el => el.EventDate)
            .ToListAsync();
    }

    public async Task AddAsync(EventLog eventLog)
    {
        _context.EventLogs.Add(eventLog);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EventLog eventLog)
    {
        EventLog? existing = await _context.EventLogs
            .FirstOrDefaultAsync(el => el.Id == eventLog.Id);

        if (existing is not null)
        {
            existing.EmotionId = eventLog.EmotionId;
            existing.Description = eventLog.Description;
            existing.Intensity = eventLog.Intensity;
            existing.EventDate = eventLog.EventDate;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(EventLog eventLog)
    {
        EventLog? existing = await _context.EventLogs
            .FirstOrDefaultAsync(el => el.Id == eventLog.Id);

        if (existing is not null)
        {
            _context.EventLogs.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
