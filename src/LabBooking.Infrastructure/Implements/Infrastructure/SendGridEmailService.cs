using LabBooking.Application.Features.Emails.Dtos;
using LabBooking.Application.Interfaces.Infrastructure;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace LabBooking.Infrastructure.Implements.Infrastructure;

public class SendGridEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SendGridEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body, EmailAttachmentDto attachment = null)
    {
        var settings = _config.GetSection("MailSettings");
        var apiKey = settings["Password"]; // Lấy API Key
        var fromEmail = settings["Mail"];  // Email đã verify trên SendGrid
        var fromName = settings["DisplayName"] ?? "LabBooking System";

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var toAddress = new EmailAddress(to);

        // Tạo nội dung email
        var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, "", body);

        // Thêm đính kèm (nếu có)
        if (attachment != null && attachment.FileContent != null)
        {
            var fileBase64 = Convert.ToBase64String(attachment.FileContent);
            msg.AddAttachment(attachment.FileName, fileBase64, attachment.ContentType);
        }

        // Gửi qua API
        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode == System.Net.HttpStatusCode.Accepted ||
            response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine($"[Success] Sent to {to} via SendGrid API");
        }
        else
        {
            // Đọc lỗi từ SendGrid trả về
            var errorBody = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid Error: {response.StatusCode} - {errorBody}");
        }
    }
}
