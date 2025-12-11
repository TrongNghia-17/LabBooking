using LabBooking.Application.Common.Dtos;

namespace LabBooking.Application.Services;

public interface IEmailTemplateService
{
    // Hàm này nhận vào "Văn bản mẫu" và "Thông tin sinh viên", trả về "Văn bản hoàn chỉnh"
    string GenerateContent(string template, StudentExcelDto student);
}
