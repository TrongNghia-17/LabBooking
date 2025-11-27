namespace LabBooking.Domain.Entities;

public class Support
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;

    public string? Answer { get; set; } = default!;

    public SupportStatus Status { get; set; } = SupportStatus.Pending;

    public Guid CreatedById { get; set; }
    [ForeignKey(nameof(CreatedById))]
    public User? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
}

public enum SupportStatus
{
    Pending,    // Chờ xử lý
    Responded,  // Admin đã trả lời
    Ignored     // Admin đánh dấu là rác/không liên quan
}
