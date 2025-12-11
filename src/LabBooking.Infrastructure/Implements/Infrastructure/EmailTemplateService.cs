using LabBooking.Application.Features.Emails.Dtos;
using LabBooking.Application.Interfaces.Infrastructure;

namespace LabBooking.Infrastructure.Implements.Infrastructure;

public class EmailTemplateService : IEmailTemplateService
{
    public string GenerateContent(string template, StudentExcelDto student)
    {
        if (string.IsNullOrEmpty(template)) return string.Empty;

        // Pattern thay thế đơn giản (có thể mở rộng thêm {{Email}}, {{Date}}...)
        var content = template
            .Replace("{{FullName}}", student.FullName)
            .Replace("{{Email}}", student.Email);

        return content;
    }
}