using DebugMeBackend.Entities;
using DebugMeBackend.Repositories.Interfaces;
using DebugMeBackend.Services;
using FluentAssertions;
using Moq;

namespace DebugMeBackend.Tests.Services
{
    public class EmotionServiceTests
    {
        // Post Tests
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
            repositoryMock
                .Setup(repository => repository.GetByNameAsync("emoção de teste"))
                .ReturnsAsync((Emotion?)null);

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

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenEmotionAlreadyExists()
        {
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            repositoryMock
                .Setup(repository => repository.GetByNameAsync("emoção existente"))
                .ReturnsAsync(new Emotion { Name = "emoção existente" });

            EmotionService service = new EmotionService(repositoryMock.Object);

            Emotion emotion = new Emotion
            {
                Name = " Emoção Existente ",
                Description = "Descrição"
            };

            Func<Task> action = async () => await service.CreateAsync(emotion);

            await action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Essa emoção já existe.");
        }

        // Get Tests
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEmotions()
        {
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            repositoryMock.Setup(repository => repository.GetAllAsync())
                .ReturnsAsync(new List<Emotion>
                {
                    new Emotion { Name = "Felicidade", Description = "Sentimento de alegria e satisfação." },
                    new Emotion { Name = "Tristeza", Description = "Sentimento de melancolia e desânimo." }
                });

            EmotionService service = new EmotionService(repositoryMock.Object);

            IEnumerable<Emotion> result = await service.GetAllAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(e => e.Name == "Felicidade" && e.Description == "Sentimento de alegria e satisfação.");
            result.Should().Contain(e => e.Name == "Tristeza" && e.Description == "Sentimento de melancolia e desânimo.");

            repositoryMock.Verify(repository => repository.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoEmotionsExist()
        {
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            repositoryMock.Setup(repository => repository.GetAllAsync())
                .ReturnsAsync(new List<Emotion>());

            EmotionService service = new EmotionService(repositoryMock.Object);

            IEnumerable<Emotion> result = await service.GetAllAsync();

            result.Should().NotBeNull();
            result.Should().BeEmpty();

            repositoryMock.Verify(repository => repository.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEmotion_WhenEmotionExists()
        {
            Guid emotionId = Guid.NewGuid();
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            repositoryMock.Setup(repository => repository.GetByIdAsync(emotionId))
                .ReturnsAsync(new Emotion { Id = emotionId, Name = "Raiva", Description = "Sentimento de irritação e fúria." });

            EmotionService service = new EmotionService(repositoryMock.Object);

            Emotion? result = await service.GetByIdAsync(emotionId);

            result.Should().NotBeNull();
            result!.Id.Should().Be(emotionId);
            result.Name.Should().Be("Raiva");
            result.Description.Should().Be("Sentimento de irritação e fúria.");

            repositoryMock.Verify(repository => repository.GetByIdAsync(emotionId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEmotionDoesNotExist()
        {
            Guid emotionId = Guid.NewGuid();
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            repositoryMock.Setup(repository => repository.GetByIdAsync(emotionId))
                .ReturnsAsync((Emotion?)null);

            EmotionService service = new EmotionService(repositoryMock.Object);

            Emotion? result = await service.GetByIdAsync(emotionId);

            result.Should().BeNull();

            repositoryMock.Verify(repository => repository.GetByIdAsync(emotionId), Times.Once);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldThrowException_WhenNameIsEmpty()
        {
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            EmotionService service = new EmotionService(repositoryMock.Object);

            Func<Task> action = async () => await service.GetByNameAsync("");

            await action.Should().ThrowAsync<ArgumentException>()
                .WithMessage("O nome da emoção é obrigatório.");
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnEmotion_WhenEmotionExists()
        {
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            repositoryMock.Setup(repository => repository.GetByNameAsync("ansiedade"))
                .ReturnsAsync(new Emotion { Name = "ansiedade", Description = "Sentimento de preocupação e apreensão." });

            EmotionService service = new EmotionService(repositoryMock.Object);

            Emotion? result = await service.GetByNameAsync(" Ansiedade ");

            result.Should().NotBeNull();
            result!.Name.Should().Be("ansiedade");
            result.Description.Should().Be("Sentimento de preocupação e apreensão.");

            repositoryMock.Verify(repository => repository.GetByNameAsync("ansiedade"), Times.Once);
        }

        // Update Tests
        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenIdIsEmpty()
        {
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            EmotionService service = new EmotionService(repositoryMock.Object);

            Emotion emotionToUpdate = new Emotion
            {
                Name = "Medo",
                Description = "Descrição atualizada"
            };

            Func<Task> action = async () => await service.UpdateAsync(Guid.Empty, emotionToUpdate);

            await action.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Id inválido.");
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenEmotionIsNull()
        {
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            EmotionService service = new EmotionService(repositoryMock.Object);

            Func<Task> action = async () => await service.UpdateAsync(Guid.NewGuid(), null!);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenNameAlreadyExists()
        {
            Guid emotionId = Guid.NewGuid();
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            repositoryMock
                .Setup(repository => repository.GetByNameAsync("surpresa atualizada"))
                .ReturnsAsync(new Emotion { Id = Guid.NewGuid(), Name = "surpresa atualizada" });

            EmotionService service = new EmotionService(repositoryMock.Object);

            Emotion emotionToUpdate = new Emotion
            {
                Name = " Surpresa Atualizada ",
                Description = "Descrição atualizada"
            };

            Func<Task> action = async () => await service.UpdateAsync(emotionId, emotionToUpdate);

            await action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Já existe outra emoção com esse nome.");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEmotion_WhenDataIsValid()
        {
            Guid emotionId = Guid.NewGuid();
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();
            repositoryMock
                .Setup(repository => repository.GetByNameAsync("surpresa atualizada"))
                .ReturnsAsync((Emotion?)null);

            EmotionService service = new EmotionService(repositoryMock.Object);

            Emotion emotionToUpdate = new Emotion
            {
                Name = " Surpresa Atualizada ",
                Description = " Descrição atualizada "
            };

            Emotion result = await service.UpdateAsync(emotionId, emotionToUpdate);

            result.Should().NotBeNull();
            result.Id.Should().Be(emotionId);
            result.Name.Should().Be("surpresa atualizada");
            result.Description.Should().Be(" Descrição atualizada ");

            repositoryMock.Verify(repository => repository.UpdateAsync(It.Is<Emotion>(e =>
                e.Id == emotionId &&
                e.Name == "surpresa atualizada" &&
                e.Description == " Descrição atualizada ")), Times.Once);
        }

        // Delete Tests
        [Fact]
        public async Task DeleteAsync_ShouldDeleteEmotion_WhenEmotionExists()
        {
            Guid emotionId = Guid.NewGuid();
            Mock<IEmotionRepository> repositoryMock = new Mock<IEmotionRepository>();

            EmotionService service = new EmotionService(repositoryMock.Object);

            await service.DeleteAsync(emotionId);

            repositoryMock.Verify(repository => repository.DeleteAsync(It.Is<Emotion>(e =>
                e.Id == emotionId)), Times.Once);
        }
    }
}
