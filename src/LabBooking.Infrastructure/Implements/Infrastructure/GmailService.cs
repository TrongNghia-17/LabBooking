using LabBooking.Application.Features.Emails.Dtos;
using LabBooking.Application.Interfaces.Infrastructure;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LabBooking.Infrastructure.Implements.Infrastructure;

public class GmailService : IEmailService
{
    private readonly IConfiguration _config;

    public GmailService(IConfiguration config) => _config = config;

    public async Task SendEmailAsync(string to, string subject, string body, EmailAttachmentDto attachment = null)
    {
        var settings = _config.GetSection("MailSettings");

        // Lấy thông tin từ cấu hình
        var host = settings["Host"];
        var port = int.Parse(settings["Port"]);
        var mail = settings["Mail"];
        var password = settings["Password"];
        var displayName = settings["DisplayName"] ?? "LabBooking System";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(displayName, mail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };
        if (attachment != null && attachment.FileContent != null)
        {
            builder.Attachments.Add(attachment.FileName, attachment.FileContent, ContentType.Parse(attachment.ContentType));
        }
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        // Timeout 30s là đủ, đừng để quá lâu gây treo request
        client.Timeout = 30000;

        try
        {
            // --- ĐOẠN SỬA QUAN TRỌNG ---
            // Tự động chọn SslOnConnect nếu port là 465, ngược lại dùng StartTls (cho 587 hoặc 2525)
            var socketOptions = port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

            // Kết nối
            Console.WriteLine($"[Email] Connecting to {host}:{port}...");
            await client.ConnectAsync(host, port, socketOptions);

            // Đăng nhập
            Console.WriteLine("[Email] Authenticating...");
            await client.AuthenticateAsync(mail, password);

            // Gửi
            Console.WriteLine($"[Email] Sending to {to}...");
            await client.SendAsync(message);

            Console.WriteLine($"[Success] Email sent to {to}");
        }
        catch (Exception ex)
        {
            // Log rõ lỗi để debug trên Render Console
            Console.WriteLine($"[Mail Error] Host: {host}, Port: {port}");
            Console.WriteLine($"[Mail Error] Exception: {ex.Message}");
            throw; // Ném lỗi ra để Hangfire biết là Job fail
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}