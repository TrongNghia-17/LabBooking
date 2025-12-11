using LabBooking.Application.Common.Dtos;
using LabBooking.Application.Services;

namespace LabBooking.Infrastructure.Services;

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