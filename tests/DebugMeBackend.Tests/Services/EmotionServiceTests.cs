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

        [Fact]
        public async Task CreateAsync_ShouldCreateEmotion_WhenNameIsProvided()
        {
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            EmotionService service = new EmotionService(repositoryMock.Object);

            Emotion emotion = new Emotion
            {
                Name = " Emoção de Teste ",
                Description = " Descrição de teste "
            };

            Emotion result = await service.CreateAsync(emotion);

            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be("emoção de teste");
            result.Description.Should().Be("Descrição de teste");

            repositoryMock.Verify(
                repository => repository.AddAsync(It.IsAny<Emotion>()),
                Times.Once);
        }
    }
}