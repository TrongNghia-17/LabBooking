using LabBooking.Application.Features.Course.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Course.Queries.GetAllCourses
{
    public record GetAllCoursesQuery(
    string? SearchPhrase,
    int PageNumber,
    int PageSize,
    string? SortBy,
    SortDirection SortDirection
    ) : IRequest<PagedResult<CourseResponse>>;
}
