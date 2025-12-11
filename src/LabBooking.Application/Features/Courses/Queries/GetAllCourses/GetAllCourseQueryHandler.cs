using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.Courses.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Courses.Queries.GetAllCourses
{
    public class GetAllCoursesQueryHandler(
    ILogger<GetAllCoursesQueryHandler> logger,
    ICourseRepository courseRepository,
    IMapper mapper) : IRequestHandler<GetAllCoursesQuery, PagedResult<CourseResponse>>
    {
        public async Task<PagedResult<CourseResponse>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting all courses with search: {SearchPhrase}", request.SearchPhrase);

            var (courses, totalCount) = await courseRepository.GetAllMatchingAsync(
                request.SearchPhrase,
                request.PageSize,
                request.PageNumber,
                request.SortBy,
                request.SortDirection);

            var coursesResponse = mapper.Map<IEnumerable<CourseResponse>>(courses);

            var result = new PagedResult<CourseResponse>(
                coursesResponse,
                totalCount,
                request.PageSize,
                request.PageNumber);

            return result;
        }
    }
}
