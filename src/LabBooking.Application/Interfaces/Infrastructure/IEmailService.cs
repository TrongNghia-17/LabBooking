using LabBooking.Application.Features.Emails.Dtos;

namespace LabBooking.Application.Interfaces.Infrastructure;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, EmailAttachmentDto attachment = null);
}
