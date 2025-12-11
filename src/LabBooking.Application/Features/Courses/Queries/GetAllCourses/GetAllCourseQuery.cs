using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.Courses.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Courses.Queries.GetAllCourses
{
    public record GetAllCoursesQuery(
    string? SearchPhrase,
    int PageNumber,
    int PageSize,
    string? SortBy,
    SortDirection SortDirection
    ) : IRequest<PagedResult<CourseResponse>>;
}
