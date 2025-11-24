namespace LabBooking.Application.Services.Notifications;

public interface INotificationService
{
    Task SendPushNotificationAsync(List<string> userPushTokens, string title, string body, object? data = null);
}
