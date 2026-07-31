using DebugMeBackend.Entities;

namespace DebugMeBackend.Repositories.Interfaces
{
    public interface IEmotionRepository
    {
        Task<Emotion?> GetByIdAsync(Guid id);
        Task<Emotion?> GetByNameAsync(string name);
        Task<Emotion?> GetByNameAndUserIdAsync(string name, Guid userId);
        Task<IEnumerable<Emotion>> GetAllAsync();
        Task<IEnumerable<Emotion>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Emotion>> GetAllWithEventCountAsync();
        Task<IEnumerable<Emotion>> GetAllWithUserEventCountAsync(Guid userId);
        Task AddAsync(Emotion emotion);
        Task UpdateAsync(Emotion emotion);
        Task DeleteAsync(Emotion emotion);
    }
}
