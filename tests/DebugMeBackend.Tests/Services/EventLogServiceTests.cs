using DebugMeBackend.Entities;
using DebugMeBackend.Repositories.Interfaces;
using DebugMeBackend.Services;
using FluentAssertions;
using Moq;

namespace DebugMeBackend.Tests.Services;

public class EventLogServiceTests
{
    private static (Mock<IEventLogRepository> EventLog, Mock<IUserRepository> User, Mock<IEmotionRepository> Emotion) CreateMocks()
    {
        return (new Mock<IEventLogRepository>(), new Mock<IUserRepository>(), new Mock<IEmotionRepository>());
    }

    private static EventLogService CreateService(
        Mock<IEventLogRepository>? eventLogMock = null,
        Mock<IUserRepository>? userMock = null,
        Mock<IEmotionRepository>? emotionMock = null)
    {
        return new EventLogService(
            (eventLogMock ?? new Mock<IEventLogRepository>()).Object,
            (userMock ?? new Mock<IUserRepository>()).Object,
            (emotionMock ?? new Mock<IEmotionRepository>()).Object);
    }

    // ========== Create ==========

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenUserIdIsEmpty()
    {
        EventLogService service = CreateService();

        Func<Task> action = () => service.CreateAsync(new EventLog
        {
            UserId = Guid.Empty,
            EmotionId = Guid.NewGuid(),
            Intensity = 5,
            EventDate = DateTime.UtcNow.AddDays(-1)
        });

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("O usuário é obrigatório.");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenEmotionIdIsEmpty()
    {
        EventLogService service = CreateService();

        Func<Task> action = () => service.CreateAsync(new EventLog
        {
            UserId = Guid.NewGuid(),
            EmotionId = Guid.Empty,
            Intensity = 5,
            EventDate = DateTime.UtcNow.AddDays(-1)
        });

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("A emoção é obrigatória.");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenIntensityIsOutOfRange()
    {
        EventLogService service = CreateService();

        Func<Task> action = () => service.CreateAsync(new EventLog
        {
            UserId = Guid.NewGuid(),
            EmotionId = Guid.NewGuid(),
            Intensity = 0,
            EventDate = DateTime.UtcNow.AddDays(-1)
        });

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("A intensidade deve estar entre 1 e 10.");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenEventDateIsFuture()
    {
        EventLogService service = CreateService();

        Func<Task> action = () => service.CreateAsync(new EventLog
        {
            UserId = Guid.NewGuid(),
            EmotionId = Guid.NewGuid(),
            Intensity = 5,
            EventDate = DateTime.UtcNow.AddDays(1)
        });

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("A data do evento não pode ser futura.");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenUserNotFound()
    {
        Guid userId = Guid.NewGuid();
        (var eventLogMock, var userMock, var emotionMock) = CreateMocks();
        userMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        EventLogService service = CreateService(eventLogMock, userMock, emotionMock);

        Func<Task> action = () => service.CreateAsync(new EventLog
        {
            UserId = userId,
            EmotionId = Guid.NewGuid(),
            Intensity = 5,
            EventDate = DateTime.UtcNow.AddDays(-1)
        });

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Usuário não encontrado.");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenEmotionNotFound()
    {
        Guid userId = Guid.NewGuid();
        Guid emotionId = Guid.NewGuid();
        (var eventLogMock, var userMock, var emotionMock) = CreateMocks();
        userMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
        emotionMock.Setup(r => r.GetByIdAsync(emotionId)).ReturnsAsync((Emotion?)null);

        EventLogService service = CreateService(eventLogMock, userMock, emotionMock);

        Func<Task> action = () => service.CreateAsync(new EventLog
        {
            UserId = userId,
            EmotionId = emotionId,
            Intensity = 5,
            EventDate = DateTime.UtcNow.AddDays(-1)
        });

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Emoção não encontrada.");
    }

    [Fact]
    public async Task CreateAsync_ShouldSucceed_WhenDataIsValid()
    {
        Guid userId = Guid.NewGuid();
        Guid emotionId = Guid.NewGuid();
        (var eventLogMock, var userMock, var emotionMock) = CreateMocks();
        userMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
        emotionMock.Setup(r => r.GetByIdAsync(emotionId)).ReturnsAsync(new Emotion { Id = emotionId });

        EventLogService service = CreateService(eventLogMock, userMock, emotionMock);

        DateTime eventDate = DateTime.UtcNow.AddDays(-1);

        EventLog result = await service.CreateAsync(new EventLog
        {
            UserId = userId,
            EmotionId = emotionId,
            Description = " Descrição de teste ",
            Intensity = 7,
            EventDate = eventDate
        });

        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.UserId.Should().Be(userId);
        result.EmotionId.Should().Be(emotionId);
        result.Description.Should().Be("Descrição de teste");
        result.Intensity.Should().Be(7);
        result.EventDate.Should().Be(eventDate);

        eventLogMock.Verify(r => r.AddAsync(It.IsAny<EventLog>()), Times.Once);
    }

    // ========== Get ==========

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEventLogs()
    {
        (var eventLogMock, var userMock, var emotionMock) = CreateMocks();
        eventLogMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<EventLog>
        {
            new EventLog { UserId = Guid.NewGuid(), EmotionId = Guid.NewGuid(), Intensity = 5, EventDate = DateTime.UtcNow },
            new EventLog { UserId = Guid.NewGuid(), EmotionId = Guid.NewGuid(), Intensity = 8, EventDate = DateTime.UtcNow }
        });

        EventLogService service = CreateService(eventLogMock, userMock, emotionMock);

        IEnumerable<EventLog> result = await service.GetAllAsync();

        result.Should().HaveCount(2);
        eventLogMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEventLog_WhenExists()
    {
        Guid id = Guid.NewGuid();
        (var eventLogMock, var userMock, var emotionMock) = CreateMocks();
        eventLogMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new EventLog { Id = id });

        EventLogService service = CreateService(eventLogMock, userMock, emotionMock);

        EventLog? result = await service.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnUserEventLogs()
    {
        Guid userId = Guid.NewGuid();
        (var eventLogMock, var userMock, var emotionMock) = CreateMocks();
        eventLogMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(new List<EventLog>
        {
            new EventLog { UserId = userId, EmotionId = Guid.NewGuid(), Intensity = 5, EventDate = DateTime.UtcNow }
        });

        EventLogService service = CreateService(eventLogMock, userMock, emotionMock);

        IEnumerable<EventLog> result = await service.GetByUserIdAsync(userId);

        result.Should().HaveCount(1);
        result.First().UserId.Should().Be(userId);
    }

    // ========== Update ==========

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenIdIsEmpty()
    {
        EventLogService service = CreateService();

        Func<Task> action = () => service.UpdateAsync(Guid.Empty, new EventLog());

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Id inválido.");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenEventLogIsNull()
    {
        EventLogService service = CreateService();

        Func<Task> action = () => service.UpdateAsync(Guid.NewGuid(), null!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenNotFound()
    {
        Guid id = Guid.NewGuid();
        (var eventLogMock, var userMock, var emotionMock) = CreateMocks();
        eventLogMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((EventLog?)null);

        EventLogService service = CreateService(eventLogMock, userMock, emotionMock);

        Func<Task> action = () => service.UpdateAsync(id, new EventLog { Intensity = 5, EventDate = DateTime.UtcNow.AddDays(-1) });

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Registro não encontrado.");
    }

    [Fact]
    public async Task UpdateAsync_ShouldSucceed_WhenDataIsValid()
    {
        Guid id = Guid.NewGuid();
        Guid emotionId = Guid.NewGuid();
        (var eventLogMock, var userMock, var emotionMock) = CreateMocks();
        eventLogMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new EventLog
        {
            Id = id,
            UserId = Guid.NewGuid(),
            EmotionId = Guid.NewGuid(),
            Intensity = 3,
            EventDate = DateTime.UtcNow.AddDays(-5)
        });
        emotionMock.Setup(r => r.GetByIdAsync(emotionId)).ReturnsAsync(new Emotion { Id = emotionId });

        EventLogService service = CreateService(eventLogMock, userMock, emotionMock);

        DateTime eventDate = DateTime.UtcNow.AddDays(-1);

        EventLog result = await service.UpdateAsync(id, new EventLog
        {
            EmotionId = emotionId,
            Description = "Atualizado",
            Intensity = 8,
            EventDate = eventDate
        });

        result.Id.Should().Be(id);
        result.EmotionId.Should().Be(emotionId);
        result.Intensity.Should().Be(8);

        eventLogMock.Verify(r => r.UpdateAsync(It.Is<EventLog>(el =>
            el.Id == id && el.EmotionId == emotionId && el.Intensity == 8)), Times.Once);
    }

    // ========== Delete ==========

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository()
    {
        Guid id = Guid.NewGuid();
        (var eventLogMock, var userMock, var emotionMock) = CreateMocks();

        EventLogService service = CreateService(eventLogMock, userMock, emotionMock);

        await service.DeleteAsync(id);

        eventLogMock.Verify(r => r.DeleteAsync(It.Is<EventLog>(el => el.Id == id)), Times.Once);
    }
}
