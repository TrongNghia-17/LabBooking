using LabBooking.Application.Features.Courses.Dtos;

namespace LabBooking.Application.Features.Courses.Queries.GetCourseById;

public class GetCourseByIdQueryHandler(
    ICourseRepository courseRepository,
    IMapper mapper
    ) : IRequestHandler<GetCourseByIdQuery, CourseResponse>
{
    public async Task<CourseResponse> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy dữ liệu từ DB
        var course = await courseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (course == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy học phần với ID: {request.Id}");
        }

        // 2. Map sang Response
        return mapper.Map<CourseResponse>(course);
    }
}
