using LabBooking.Application.Features.Courses.Dtos;

namespace LabBooking.Application.Features.Courses.Queries.GetCourseById;

public record GetCourseByIdQuery(Guid Id) : IRequest<CourseResponse>;
