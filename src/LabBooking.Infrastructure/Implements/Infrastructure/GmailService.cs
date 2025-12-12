using LabBooking.Application.Features.Emails.Dtos;
using LabBooking.Application.Interfaces.Infrastructure;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LabBooking.Infrastructure.Implements.Infrastructure
{
    public class GmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public GmailService(IConfiguration config) => _config = config;

        public async Task SendEmailAsync(string to, string subject, string body, EmailAttachmentDto attachment = null)
        {
            var settings = _config.GetSection("MailSettings");

            var host = settings["Host"];
            var port = int.Parse(settings["Port"]);
            var mailAddress = settings["Mail"];
            var password = settings["Password"];
            var displayName = settings["DisplayName"] ?? "LabBooking System";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(displayName, mailAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            if (attachment != null && attachment.FileContent != null)
            {
                builder.Attachments.Add(attachment.FileName, attachment.FileContent, ContentType.Parse(attachment.ContentType));
            }
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            // Tăng timeout lên 60s để tránh lỗi mạng trên Render
            client.Timeout = 60000;

            try
            {
                // CẤU HÌNH CHO GMAIL
                // Port 587: Dùng SecureSocketOptions.StartTls
                // Port 465: Dùng SecureSocketOptions.SslOnConnect

                SecureSocketOptions socketOptions;
                if (port == 465)
                    socketOptions = SecureSocketOptions.SslOnConnect;
                else
                    socketOptions = SecureSocketOptions.StartTls;

                await client.ConnectAsync(host, port, socketOptions);

                // Đăng nhập
                await client.AuthenticateAsync(mailAddress, password);

                // Gửi
                await client.SendAsync(message);
                Console.WriteLine($"[Success] Email sent to {to} via Gmail");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Mail Error] Host: {host} | Port: {port} | Error: {ex.Message}");
                throw;
            }
            finally
            {
                if (client.IsConnected) await client.DisconnectAsync(true);
            }
        }
    }
}