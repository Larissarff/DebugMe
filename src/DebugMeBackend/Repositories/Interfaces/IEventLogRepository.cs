using DebugMeBackend.Entities;

namespace DebugMeBackend.Repositories.Interfaces;

public interface IEventLogRepository
{
    Task<EventLog?> GetByIdAsync(Guid id);
    Task<IEnumerable<EventLog>> GetAllAsync();
    Task<IEnumerable<EventLog>> GetByUserIdAsync(Guid userId);
    Task AddAsync(EventLog eventLog);
    Task UpdateAsync(EventLog eventLog);
    Task DeleteAsync(EventLog eventLog);
}
