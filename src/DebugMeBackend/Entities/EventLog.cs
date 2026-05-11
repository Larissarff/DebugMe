namespace DebugMeBackend.Entities;

public class EventLog
{
    public EventLog()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid EmotionId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Intensity { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public Emotion Emotion { get; set; } = null!;
}
