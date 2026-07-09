namespace DebugMeBackend.DTOs.EventLog;

public class EventLogResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid EmotionId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Intensity { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public EmotionInfoDto? Emotion { get; set; }
    public UserInfoDto? User { get; set; }
}

public class EmotionInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
