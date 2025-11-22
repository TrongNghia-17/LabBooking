using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Courses.Dtos
{
    public record CourseResponse
    (
        Guid Id,
        string? CourseCode,
        string? CourseName,
        string? Description,
        DateTime? CreatedDate,
        bool? IsActive
    );
}
