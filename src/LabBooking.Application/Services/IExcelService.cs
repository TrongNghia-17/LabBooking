using LabBooking.Application.Common.Dtos;

namespace LabBooking.Application.Services;

public interface IExcelService
{
    List<StudentExcelDto> ReadStudents(Stream stream);
}
