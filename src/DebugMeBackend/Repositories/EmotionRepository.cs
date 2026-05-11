using DebugMeBackend.Data;
using DebugMeBackend.Entities;
using DebugMeBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DebugMeBackend.Repositories
{
    public class EmotionRepository : IEmotionRepository
    {
        private readonly AppDbContext _context;

        public EmotionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Emotion>> GetAllAsync()
        {
            return await _context.Emotions.ToListAsync();
        }

        public async Task<Emotion?> GetByIdAsync(Guid id)
        {
            return await _context.Emotions.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Emotion?> GetByNameAsync(string name)
        {
            return await _context.Emotions.FirstOrDefaultAsync(e => e.Name == name);
        }

        public async Task AddAsync(Emotion emotion)
        {
            _context.Emotions.Add(emotion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Emotion emotion)
        {
            Emotion? existing = await _context.Emotions
                .FirstOrDefaultAsync(e => e.Id == emotion.Id);

            if (existing is not null)
            {
                existing.Name = emotion.Name;
                existing.Description = emotion.Description;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Emotion emotion)
        {
            Emotion? existing = await _context.Emotions
                .FirstOrDefaultAsync(e => e.Id == emotion.Id);

            if (existing is not null)
            {
                _context.Emotions.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
