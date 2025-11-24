namespace LabBooking.Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommandHandler(
    ILogger<CreateCourseCommandHandler> logger,
    IMapper mapper,
    ICourseRepository courseRepository
    ) : IRequestHandler<CreateCourseCommand, Guid>
{
    public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang tạo Course mới: {CourseCode} - {CourseName}", request.CourseCode, request.CourseName);

        // 1. Map dữ liệu
        var course = mapper.Map<Course>(request);

        // 2. Gán các giá trị mặc định
        course.CreatedDate = DateTime.UtcNow;
        course.IsActive = true;

        // 3. Lưu vào DB
        var courseId = await courseRepository.Create(course, cancellationToken);

        logger.LogInformation("Tạo Course thành công với ID: {CourseId}", courseId);

        return courseId;
    }
}
