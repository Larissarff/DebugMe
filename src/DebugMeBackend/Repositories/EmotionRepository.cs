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

        public async Task<IEnumerable<Emotion>> GetAllWithEventCountAsync()
        {
            return await _context.Emotions
                .Include(e => e.EventLogs)
                .ToListAsync();
        }

        public async Task<IEnumerable<Emotion>> GetAllWithUserEventCountAsync(Guid userId)
        {
            var result = await _context.Emotions
                .Where(e => e.UserId == null || e.UserId == userId)
                .Select(e => new
                {
                    Emotion = e,
                    EventCount = e.EventLogs.Count(el => el.UserId == userId)
                })
                .ToListAsync();

            return result.Select(x =>
            {
                return new Emotion
                {
                    Id = x.Emotion.Id,
                    Name = x.Emotion.Name,
                    Description = x.Emotion.Description,
                    CreatedAt = x.Emotion.CreatedAt,
                    UpdatedAt = x.Emotion.UpdatedAt,
                    UserId = x.Emotion.UserId,
                    EventLogs = Enumerable.Range(0, x.EventCount)
                        .Select(_ => new EventLog())
                        .ToList()
                };
            }).ToList();
        }

        public async Task<Emotion?> GetByIdAsync(Guid id)
        {
            return await _context.Emotions.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Emotion?> GetByNameAsync(string name)
        {
            return await _context.Emotions.FirstOrDefaultAsync(e => e.Name == name);
        }

        public async Task<Emotion?> GetByNameAndUserIdAsync(string name, Guid userId)
        {
            return await _context.Emotions.FirstOrDefaultAsync(e => e.Name == name && e.UserId == userId);
        }

        public async Task<IEnumerable<Emotion>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Emotions
                .Where(e => e.UserId == null || e.UserId == userId)
                .ToListAsync();
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
