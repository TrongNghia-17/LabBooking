using LabBooking.Application.Services.Notifications;
using System.Net.Http.Json;

namespace LabBooking.Infrastructure.Services.Notifications;

public class NotificationService : INotificationService

{
    private readonly IHttpClientFactory _httpClientFactory;
    private const string ExpoPushApiUrl = "https://exp.host/--/api/v2/push/send";

    public NotificationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendPushNotificationAsync(
        List<string> userPushTokens,
        string title,
        string body,
        object? data = null)
    {
        if (!userPushTokens.Any()) return;

        var client = _httpClientFactory.CreateClient();
        var payload = new
        {
            to = userPushTokens,
            title = title,
            body = body,
            sound = "default",
            data = data
        };

        var response = await client.PostAsJsonAsync(ExpoPushApiUrl, payload);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Lỗi gửi thông báo: {error}");
        }
    }
}
