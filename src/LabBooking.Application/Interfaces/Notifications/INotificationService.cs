namespace LabBooking.Application.Interfaces.Notifications;

public interface INotificationService
{
    Task SendPushNotificationAsync(List<string> userPushTokens, string title, string body, object? data = null);
}
