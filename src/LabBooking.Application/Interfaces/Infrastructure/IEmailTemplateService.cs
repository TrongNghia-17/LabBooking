using LabBooking.Application.Features.Emails.Dtos;

namespace LabBooking.Application.Interfaces.Infrastructure;

public interface IEmailTemplateService
{
    // Hàm này nhận vào "Văn bản mẫu" và "Thông tin sinh viên", trả về "Văn bản hoàn chỉnh"
    string GenerateContent(string template, StudentExcelDto student);
}
