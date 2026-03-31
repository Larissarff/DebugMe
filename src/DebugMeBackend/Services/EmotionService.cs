using DebugMeBackend.Entities;
using DebugMeBackend.Repositories.Interfaces;

namespace DebugMeBackend.Services
{
    public class EmotionService
    {
        private readonly IEmotionRepository _emotionRepository;

        public EmotionService(IEmotionRepository emotionRepository)
        {
            _emotionRepository = emotionRepository;
        }

        public async Task<Emotion> CreateAsync(Emotion emotion)
        {
            if (string.IsNullOrWhiteSpace(emotion.Name))
            {
                throw new ArgumentException("O nome da emoção é obrigatório.");
            }

            string normalizedName = emotion.Name.Trim().ToLower();

            Emotion? existingEmotion = await _emotionRepository.GetByNameAsync(normalizedName);

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

            await _emotionRepository.AddAsync(newEmotion);

            return newEmotion;
        }

        public async Task<Emotion?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id inválido.");
            }

            return await _emotionRepository.GetByIdAsync(id);
        }

        public async Task<List<Emotion>> GetAllAsync()
        {
            return await _emotionRepository.GetAllAsync();
        }

        public async Task<Emotion> UpdateAsync(Guid id, string name, string? description)
        {
            Emotion? emotion = await _emotionRepository.GetByIdAsync(id);

            if (emotion is null)
            {
                throw new InvalidOperationException("Emoção não encontrada.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("O nome da emoção é obrigatório.");
            }

            string normalizedName = name.Trim().ToLower();

            Emotion? existingEmotion = await _emotionRepository.GetByNameAsync(normalizedName);

            if (existingEmotion is not null && existingEmotion.Id != id)
            {
                throw new InvalidOperationException("Já existe outra emoção com esse nome.");
            }

            emotion.Name = normalizedName;
            emotion.Description = description?.Trim();

            await _emotionRepository.UpdateAsync(emotion);

            return emotion;
        }

        public async Task DeleteAsync(Guid id)
        {
            Emotion? emotion = await _emotionRepository.GetByIdAsync(id);

            if (emotion is null)
            {
                throw new InvalidOperationException("Emoção não encontrada.");
            }

            await _emotionRepository.DeleteAsync(emotion);
        }
    }
}