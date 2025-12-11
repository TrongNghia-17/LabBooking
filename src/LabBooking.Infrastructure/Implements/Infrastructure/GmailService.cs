using LabBooking.Application.Features.Emails.Dtos;
using LabBooking.Application.Interfaces.Infrastructure;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net;
using System.Net.Sockets;

namespace LabBooking.Infrastructure.Implements.Infrastructure
{
    public class GmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public GmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body, EmailAttachmentDto attachment = null)
        {
            var settings = _config.GetSection("MailSettings");
            var host = settings["Host"];
            var port = int.Parse(settings["Port"]);
            var mail = settings["Mail"];
            var password = settings["Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings["DisplayName"] ?? "LabBooking System", mail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };

            if (attachment != null && attachment.FileContent != null)
            {
                builder.Attachments.Add(attachment.FileName, attachment.FileContent, ContentType.Parse(attachment.ContentType));
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            client.Timeout = 30000; // 30s timeout

            // --- BƯỚC FIX LỖI SSL HANDSHAKE ---
            // Chấp nhận mọi chứng chỉ (Vì ta đang kết nối bằng IP nên chứng chỉ sẽ không khớp domain)
            client.CheckCertificateRevocation = false;
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            // ----------------------------------

            try
            {
                // Logic tìm IPv4 để né lỗi trên Render
                var ipAddresses = await Dns.GetHostAddressesAsync(host);
                var ipV4 = ipAddresses.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

                var socketOptions = port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

                if (ipV4 != null)
                {
                    // Kết nối bằng IP v4
                    await client.ConnectAsync(ipV4.ToString(), port, socketOptions);
                }
                else
                {
                    // Fallback về Hostname
                    await client.ConnectAsync(host, port, socketOptions);
                }

                await client.AuthenticateAsync(mail, password);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MailKit Error] {ex.Message}");
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}