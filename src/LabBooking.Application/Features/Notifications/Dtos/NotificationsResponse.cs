namespace LabBooking.Application.Features.Notifications.Dtos;

/// <summary>
/// Represents the data transfer object for a notification.
/// </summary>
public record NotificationsResponse(
    Guid Id,
    string Title,
    string Message,
    string? DataPayload,
    bool IsRead,
    DateTime CreatedAt
);
