namespace LabBooking.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseCommandHandler(
    ILogger<UpdateCourseCommandHandler> logger,
    IMapper mapper,
    ICourseRepository courseRepository
    ) : IRequestHandler<UpdateCourseCommand>
{
    public async Task Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        // 1. Lấy entity từ DB
        var courseToUpdate = await courseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (courseToUpdate == null)
        {
            logger.LogWarning("Không tìm thấy Course với ID: {CourseId}", request.Id);
            throw new KeyNotFoundException($"Không tìm thấy học phần với ID {request.Id}");
        }

        // 2. Map dữ liệu mới vào entity cũ (AutoMapper sẽ ghi đè các field trùng tên)
        mapper.Map(request, courseToUpdate);

        // 3. Lưu thay đổi
        await courseRepository.UpdateAsync(courseToUpdate, cancellationToken);

        logger.LogInformation("Đã cập nhật Course {CourseId} thành công.", request.Id);
    }
}
