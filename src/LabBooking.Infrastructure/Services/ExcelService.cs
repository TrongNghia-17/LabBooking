using LabBooking.Application.Common.Dtos;
using LabBooking.Application.Services;
using OfficeOpenXml;

namespace LabBooking.Infrastructure.Services;

public class ExcelService : IExcelService
{
    public List<StudentExcelDto> ReadStudents(Stream stream)
    {
        var results = new List<StudentExcelDto>();

        using (var package = new ExcelPackage(stream))
        {
            var sheet = package.Workbook.Worksheets[0];
            if (sheet == null) return results;

            var rowCount = sheet.Dimension.Rows;
            for (int row = 2; row <= rowCount; row++) // Giả sử dòng 1 là Header
            {
                var stt = sheet.Cells[row, 1].Value?.ToString(); // Cột STT (nếu cần log)
                var name = sheet.Cells[row, 2].Value?.ToString()?.Trim(); // Cột 2 là Tên
                var email = sheet.Cells[row, 3].Value?.ToString()?.Trim(); // Cột 3 là Email

                if (!string.IsNullOrEmpty(email) && email.Contains("@") && email.Contains("."))
                {
                    results.Add(new StudentExcelDto { FullName = name, Email = email });
                }
            }
        }
        return results;
    }
}