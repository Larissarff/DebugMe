using DebugMeBackend.Entities;
using DebugMeBackend.Repositories.Interfaces;

namespace DebugMeBackend.Services;

public class EventLogService
{
    private readonly IEventLogRepository _eventLogRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmotionRepository _emotionRepository;

    public EventLogService(
        IEventLogRepository eventLogRepository,
        IUserRepository userRepository,
        IEmotionRepository emotionRepository)
    {
        _eventLogRepository = eventLogRepository;
        _userRepository = userRepository;
        _emotionRepository = emotionRepository;
    }

    public async Task<EventLog> CreateAsync(EventLog eventLog)
    {
        if (eventLog.UserId == Guid.Empty)
            throw new ArgumentException("O usuário é obrigatório.");

        if (eventLog.EmotionId == Guid.Empty)
            throw new ArgumentException("A emoção é obrigatória.");

        if (eventLog.Intensity is < 1 or > 10)
            throw new ArgumentException("A intensidade deve estar entre 1 e 10.");

        if (eventLog.EventDate == default)
            throw new ArgumentException("A data do evento é obrigatória.");

        if (eventLog.EventDate > DateTime.UtcNow)
            throw new ArgumentException("A data do evento não pode ser futura.");

        User? user = await _userRepository.GetByIdAsync(eventLog.UserId);
        if (user is null)
            throw new InvalidOperationException("Usuário não encontrado.");

        Emotion? emotion = await _emotionRepository.GetByIdAsync(eventLog.EmotionId);
        if (emotion is null)
            throw new InvalidOperationException("Emoção não encontrada.");

        EventLog newEventLog = new EventLog
        {
            Id = Guid.NewGuid(),
            UserId = eventLog.UserId,
            EmotionId = eventLog.EmotionId,
            Description = eventLog.Description?.Trim() ?? string.Empty,
            Intensity = eventLog.Intensity,
            EventDate = eventLog.EventDate,
            CreatedAt = DateTime.UtcNow
        };

        await _eventLogRepository.AddAsync(newEventLog);
        return newEventLog;
    }

    public async Task<EventLog?> GetByIdAsync(Guid id)
    {
        return await _eventLogRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<EventLog>> GetAllAsync()
    {
        return await _eventLogRepository.GetAllAsync();
    }

    public async Task<IEnumerable<EventLog>> GetByUserIdAsync(Guid userId)
    {
        return await _eventLogRepository.GetByUserIdAsync(userId);
    }

    public async Task<EventLog> UpdateAsync(Guid id, EventLog eventLogEdited)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id inválido.");

        if (eventLogEdited is null)
            throw new ArgumentNullException(nameof(eventLogEdited));

        EventLog? existing = await _eventLogRepository.GetByIdAsync(id);
        if (existing is null)
            throw new InvalidOperationException("Registro não encontrado.");

        if (eventLogEdited.Intensity is < 1 or > 10)
            throw new ArgumentException("A intensidade deve estar entre 1 e 10.");

        if (eventLogEdited.EventDate > DateTime.UtcNow)
            throw new ArgumentException("A data do evento não pode ser futura.");

        if (eventLogEdited.EmotionId != Guid.Empty && eventLogEdited.EmotionId != existing.EmotionId)
        {
            Emotion? emotion = await _emotionRepository.GetByIdAsync(eventLogEdited.EmotionId);
            if (emotion is null)
                throw new InvalidOperationException("Emoção não encontrada.");
        }

        eventLogEdited.Id = id;
        await _eventLogRepository.UpdateAsync(eventLogEdited);
        return eventLogEdited;
    }

    public async Task DeleteAsync(Guid id)
    {
        EventLog eventLog = new EventLog { Id = id };
        await _eventLogRepository.DeleteAsync(eventLog);
    }
}
