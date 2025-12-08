namespace LabBooking.Application.Features.Notifications.Dtos;

/// <summary>
/// Represents the data transfer object for a notification.
/// </summary>
public class NotificationsResponse // Đổi record -> class (hoặc giữ record nhưng khai báo { get; set; })
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    // 👇 QUAN TRỌNG: Phải có { get; set; } để Handler có thể ghi đè dữ liệu
    public string? DataPayload { get; set; }

    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
