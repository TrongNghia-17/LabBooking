using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Interfaces.Notifications;
using System.Text.Json;

namespace LabBooking.Application.Features.Notifications.Commands.SendNotificationToSelf;

public class SendNotificationToSelfCommandHandler(
    ILogger<SendNotificationToSelfCommandHandler> logger,
    ICurrentUserService currentUserService,
    IUserDeviceRepository userDeviceRepository,
    INotificationService notificationService,
    INotificationRepository notificationRepository)
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

        var newNotification = new Notification
        {
            Title = title,
            Message = body,
            UserId = guidUserId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            DataPayload = JsonSerializer.Serialize(data)
        };

        try
        {
            await notificationRepository.CreateAsync(newNotification, cancellationToken);
            logger.LogInformation("Test message saved to database for user {UserId}", guidUserId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving test message to database for user {UserId}", guidUserId);
        }

        await notificationService.SendPushNotificationAsync(tokens, title, body, data);

        logger.LogInformation("Sent test notification to {Count} devices for user {UserId}", tokens.Count, guidUserId);

        return $"Sent test message to {tokens.Count} devices.";
    }
}
