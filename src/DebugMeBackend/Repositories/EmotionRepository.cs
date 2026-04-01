using DebugMeBackend.Entities;

namespace DebugMeBackend.Repositories
{
    public class EmotionRepository
    {
        private readonly List<Emotion> _emotions;

        public EmotionRepository()
        {
            _emotions = new List<Emotion>();
        }

        public Task<IEnumerable<Emotion>> GetAllAsync()
        {
            return Task.FromResult(_emotions.AsEnumerable());
        }

        public Task<Emotion?> GetByIdAsync(Guid id)
        {
            Emotion? emotion = _emotions.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(emotion);
        }

        public Task<Emotion?> GetByNameAsync(string name)
        {
            Emotion? emotion = _emotions.FirstOrDefault(e => e.Name == name);
            return Task.FromResult(emotion);
        }

        public Task AddAsync(Emotion emotion)
        {
            _emotions.Add(emotion);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Emotion emotion)
        {
            Emotion? existingEmotion = _emotions.FirstOrDefault(e => e.Id == emotion.Id);

            if (existingEmotion is not null)
            {
                existingEmotion.Name = emotion.Name;
                existingEmotion.Description = emotion.Description;
                existingEmotion.UpdatedAt = DateTime.UtcNow;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            Emotion? emotion = _emotions.FirstOrDefault(e => e.Id == id);

            if (emotion is not null)
            {
                _emotions.Remove(emotion);
            }

            return Task.CompletedTask;
        }
    }
}