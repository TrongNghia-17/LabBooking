namespace LabBooking.Application.Features.Courses.Commands.DeleteCourse;

public class DeleteCourseCommandHandler(
    ILogger<DeleteCourseCommandHandler> logger,
    ICourseRepository courseRepository
    ) : IRequestHandler<DeleteCourseCommand>
{
    public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra tồn tại
        var courseToDelete = await courseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (courseToDelete == null)
        {
            logger.LogWarning("Cố gắng xóa Course không tồn tại. ID: {CourseId}", request.Id);
            throw new KeyNotFoundException($"Không tìm thấy học phần với ID: {request.Id}");
        }

        // 2. Thực hiện xóa
        // Lưu ý: Nếu sau này có bảng Lớp học (Class) hoặc Booking liên kết với Course,
        // bạn cần kiểm tra ràng buộc trước khi xóa (hoặc để Database báo lỗi Foreign Key).
        await courseRepository.DeleteAsync(courseToDelete, cancellationToken);

        logger.LogInformation("Đã xóa thành công Course có ID: {CourseId}", request.Id);
    }
}
