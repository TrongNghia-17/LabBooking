namespace LabBooking.Application.Features.Courses.Commands.CreateCourse;

public record CreateCourseCommand(
    string CourseCode,
    string CourseName,
    string? Description
) : IRequest<Guid>;
