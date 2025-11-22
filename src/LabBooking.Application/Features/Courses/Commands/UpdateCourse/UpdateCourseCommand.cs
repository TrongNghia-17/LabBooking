using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseCommand : IRequest
{
    [JsonIgnore] // ID sẽ lấy từ URL, không cần gửi trong Body JSON
    public Guid Id { get; set; }

    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } // Cho phép cập nhật trạng thái hoạt động
}
