using DebugMeBackend.DTOs.User;
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
            // Arrange
            var repositoryMock = new Mock<IEmotionRepository>();
            var service = new EmotionService(repositoryMock.Object);

            var dto = new CreateEmotionDto
            {
                Name = ""
            };

            // Act
            Func<Task> act = async () => await service.CreateAsync(dto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("O nome da emoção é obrigatório.");
        }
    }

}