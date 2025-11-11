namespace LabBooking.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? DataPayload { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
