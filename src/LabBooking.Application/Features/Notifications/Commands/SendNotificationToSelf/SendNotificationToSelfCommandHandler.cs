using LabBooking.Application.Services.Notifications;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.Notifications.Commands.SendNotificationToSelf;

public class SendNotificationToSelfCommandHandler(
    ILogger<SendNotificationToSelfCommandHandler> logger,
    ICurrentUserService currentUserService,
    IUserDeviceRepository userDeviceRepository,
    INotificationService notificationService)
    : IRequestHandler<SendNotificationToSelfCommand, string>
{
    public async Task<string> Handle(SendNotificationToSelfCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId == null)
        {
            logger.LogWarning("SendTestNotification: User is not authenticated.");
            throw new UnauthorizedAccessException("Invalid user.");
        }

        var guidUserId = userId.Value;
        var tokens = await userDeviceRepository.GetTokensByUserIdAsync(guidUserId, cancellationToken);
        if (tokens == null || !tokens.Any())
        {
            logger.LogWarning("User {UserId} has no registered device tokens.", guidUserId);
            throw new KeyNotFoundException("No push token found for this user.");
        }

        var title = "🔔 Test Thành Công!";
        var body = $"Đây là thông báo test cho user ID: {guidUserId}.";
        var data = new { bookingId = 999 };

        await notificationService.SendPushNotificationAsync(tokens, title, body, data);

        logger.LogInformation("Sent test notification to {Count} devices for user {UserId}", tokens.Count, guidUserId);

        return $"Đã gửi thông báo test đến {tokens.Count} thiết bị.";
    }
}
