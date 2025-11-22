namespace LabBooking.Application.Features.Courses.Commands.DeleteCourse;

public record DeleteCourseCommand(Guid Id) : IRequest;
