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
            return await _emotionRepository.GetByIdAsync(id);
        }

        public async Task<Emotion?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("O nome da emoção é obrigatório.");
            }

            string normalizedName = name.Trim().ToLower();

            return await _emotionRepository.GetByNameAsync(normalizedName);
        }

        public async Task<IEnumerable<Emotion>> GetAllAsync()
        {
            return await _emotionRepository.GetAllAsync();
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

            if (!string.IsNullOrWhiteSpace(emotionEdited.Name))
            {
                string normalizedName = emotionEdited.Name.Trim().ToLower();

                Emotion? existingEmotion = await _emotionRepository.GetByNameAsync(normalizedName);

                if (existingEmotion is not null && existingEmotion.Id != id)
                {
                    throw new InvalidOperationException("Já existe outra emoção com esse nome.");
                }

                emotionEdited.Name = normalizedName;
            }

            emotionEdited.Id = id;

            await _emotionRepository.UpdateAsync(emotionEdited);

            return emotionEdited;
        }

        public async Task DeleteAsync(Guid id)
        {
            Emotion emotion = new Emotion { Id = id };
            await _emotionRepository.DeleteAsync(emotion);
        }
    }
}
