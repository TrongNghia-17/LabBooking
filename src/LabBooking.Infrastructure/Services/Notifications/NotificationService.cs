using LabBooking.Application.Services.Notifications;
using System.Net.Http.Json;

namespace LabBooking.Infrastructure.Services.Notifications;

public class NotificationService(
    IHttpClientFactory httpClientFactory,
    ILogger<NotificationService> logger) : INotificationService

{
    private const string ExpoPushApiUrl = "https://exp.host/--/api/v2/push/send";

    public async Task SendPushNotificationAsync(
        List<string> userPushTokens,
        string title,
        string body,
        object? data = null)
    {
        if (!userPushTokens.Any()) return;

        var client = httpClientFactory.CreateClient();
        var payload = new
        {
            to = userPushTokens,
            title,
            body,
            sound = "default",
            data
        };

        var response = await client.PostAsJsonAsync(ExpoPushApiUrl, payload);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            logger.LogError(
                "Error sending push notification to Expo. Status: {StatusCode}. Response: {Error}",
                response.StatusCode,
                error);

            throw new HttpRequestException(
                $"Sending notification failed. Status: {response.StatusCode}. Details: {error}",
                null,
                response.StatusCode);
        }
    }
}
