using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.Notifications.Commands.MarkNotificationAsRead;

/// <summary>
/// Handles the <see cref="MarkNotificationAsReadCommand"/>.
/// </summary>
public class MarkNotificationAsReadCommandHandler(
    ILogger<MarkNotificationAsReadCommandHandler> logger,
    ICurrentUserService currentUserService,
    INotificationRepository notificationRepository
) : IRequestHandler<MarkNotificationAsReadCommand, Unit>
{
    public async Task<Unit> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId == null)
        {
            logger.LogWarning("MarkAsRead: User is not authenticated.");
            throw new UnauthorizedAccessException("Invalid user.");
        }

        var notification = await notificationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (notification == null)
        {
            logger.LogWarning("MarkAsRead: Notification not found. ID: {NotificationId}", request.Id);
            throw new KeyNotFoundException($"Notification with ID {request.Id} not found.");
        }

        if (notification.UserId != userId.Value)
        {
            logger.LogWarning(
                "MarkAsRead: User {UserId} attempted to access notification {NotificationId} belonging to user {OwnerId}.",
                userId.Value, request.Id, notification.UserId);

            throw new UnauthorizedAccessException("You do not have permission to modify this notification.");
        }

        if (notification.IsRead)
        {
            return Unit.Value;
        }

        notification.IsRead = true;
        await notificationRepository.UpdateAsync(notification, cancellationToken);

        logger.LogInformation("Notification {NotificationId} marked as read for user {UserId}",
            request.Id, userId.Value);

        return Unit.Value;
    }
}
