using LabBooking.Application.Features.Emails.Dtos;

namespace LabBooking.Application.Interfaces.Infrastructure;

public interface IExcelService
{
    List<StudentExcelDto> ReadStudents(Stream stream);
}
