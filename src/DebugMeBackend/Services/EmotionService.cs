using DebugMeBackend.Entities;
using Microsoft.EntityFrameworkCore;
using DebugMeBackend.Data;
using DebugMeBackend.Repositories.Interfaces;

namespace DebugMeBackend.Services
{
    public class EmotionService
    {
        private readonly AppDbContext _context;

        public EmotionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Emotion> CreateAsync(Emotion emotion)
        {
            if (string.IsNullOrWhiteSpace(emotion.Name))
            {
                throw new ArgumentException("O nome da emoção é obrigatório.");
            }

            string normalizedName = emotion.Name.Trim().ToLower();

            Emotion? existingEmotion = await _context.Emotions.FirstOrDefaultAsync(e => e.Name == normalizedName);

            if (existingEmotion is not null)
            {
                throw new InvalidOperationException("Essa emoção já existe.");
            }

            Emotion newEmotion = new Emotion
            {
                Id = Guid.NewGuid(),
                Name = normalizedName,
                Description = emotion.Description?.Trim()
            };

            await _context.AddAsync(newEmotion);
            await _context.SaveChangesAsync();

            return newEmotion;
        }

        public async Task<Emotion?> GetByIdAsync(Guid id)
        {
            Emotion? emotion = await _context.Emotions.FirstOrDefaultAsync(e => e.Id == id);
            return emotion;
        }
        public async Task<Emotion?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("O nome da emoção é obrigatório.");
            }

            string normalizedName = name.Trim().ToLower();

            Emotion? emotion = await _context.Emotions
                .FirstOrDefaultAsync(e => e.Name == normalizedName);

            return emotion;
        }

        public async Task<IEnumerable<Emotion>> GetAllAsync()
        {
            List<Emotion> emotions = await _context.Emotions.ToListAsync();
            return emotions;
        }

        public async Task<Emotion> UpdateAsync(Guid id, Emotion emotionEdited)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id inválido.");
            }

            if (emotionEdited is null)
            {
                throw new ArgumentNullException(nameof(emotionEdited));
            }

            Emotion? emotion = await _context.Emotions.FirstOrDefaultAsync(e => e.Id == id);

            if (emotion is null)
            {
                throw new InvalidOperationException("Emoção não encontrada.");
            }

            if (!string.IsNullOrWhiteSpace(emotionEdited.Name))
            {
                string normalizedName = emotionEdited.Name.Trim().ToLower();

                Emotion? existingEmotion = await _context.Emotions
                    .FirstOrDefaultAsync(e => e.Name == normalizedName);

                if (existingEmotion is not null && existingEmotion.Id != id)
                {
                    throw new InvalidOperationException("Já existe outra emoção com esse nome.");
                }

                emotion.Name = normalizedName;
            }

            if (emotionEdited.Description is not null)
            {
                emotion.Description = emotionEdited.Description.Trim();
            }

            await _context.SaveChangesAsync();

            return emotion;
        }

        public async Task DeleteAsync(Guid id)
        {
            Emotion? emotion = await _context.Emotions.FirstOrDefaultAsync(e => e.Id == id);

            if (emotion is null)
            {
                throw new InvalidOperationException("Emoção não encontrada.");
            }

            _context.Emotions.Remove(emotion);
            await _context.SaveChangesAsync();
        }
    }
}