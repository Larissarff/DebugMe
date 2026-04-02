using DebugMeBackend.Entities;

namespace DebugMeBackend.Repositories.Interfaces
{
    public interface IEmotionRepository
    {
        Task<Emotion?> GetByIdAsync(Guid id);
        Task<Emotion?> GetByNameAsync(string name);
        Task<IEnumerable<Emotion>> GetAllAsync();
        Task AddAsync(Emotion emotion);
        Task UpdateAsync(Emotion emotion);
        Task DeleteAsync(Guid id);
    }
}