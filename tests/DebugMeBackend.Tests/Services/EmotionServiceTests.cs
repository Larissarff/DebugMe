using DebugMeBackend.Entities;
using DebugMeBackend.Repositories.Interfaces;
using DebugMeBackend.Services;
using FluentAssertions;
using Moq;

namespace DebugMeBackend.Tests.Services
{
    public class EmotionServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenNameIsEmpty()
        {
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            EmotionService service = new EmotionService(repositoryMock.Object);

            Emotion emotion = new Emotion
            {
                Name = "",
                Description = "Descrição de teste"
            };

            Func<Task> action = async () => await service.CreateAsync(emotion);

            await action.Should().ThrowAsync<ArgumentException>()
                .WithMessage("O nome da emoção é obrigatório.");
        }
    }
}