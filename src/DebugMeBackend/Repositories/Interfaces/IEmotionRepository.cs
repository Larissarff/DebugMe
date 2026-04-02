using DebugMeBackend.Entities;

public interface IEmotionRepository
{
    Task<Emotion?> GetByIdAsync(Guid id);
    Task<Emotion?> GetByNameAsync(string name);
    Task<List<Emotion>> GetAllAsync();
    Task AddAsync(Emotion emotion);
    Task UpdateAsync(Emotion emotion);
    Task DeleteAsync(Emotion emotion);
}