namespace LabBooking.Application.Features.Notifications.Commands.MarkNotificationAsRead;

/// <summary>
/// Represents the command to mark a specific notification as read.
/// </summary>
/// <param name="Id">The unique identifier of the notification to mark as read.</param>
public record MarkNotificationAsReadCommand(
    Guid Id
) : IRequest<Unit>;
