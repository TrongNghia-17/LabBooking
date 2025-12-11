using LabBooking.Application.Common.Dtos;

namespace LabBooking.Application.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, EmailAttachmentDto attachment = null);
}
