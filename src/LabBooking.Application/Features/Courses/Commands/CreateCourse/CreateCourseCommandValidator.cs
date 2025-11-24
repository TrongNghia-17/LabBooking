namespace LabBooking.Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    private readonly ICourseRepository _courseRepository;

    public CreateCourseCommandValidator(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;

        RuleFor(c => c.CourseCode)
            .NotEmpty()
            .WithMessage("Mã học phần không được để trống.")
            .MaximumLength(20)
            .WithMessage("Mã học phần không được vượt quá 20 ký tự.")
            .MustAsync(BeUniqueCourseCode)
            .WithMessage("Mã học phần này đã tồn tại trong hệ thống.");

        RuleFor(c => c.CourseName)
            .NotEmpty()
            .WithMessage("Tên học phần không được để trống.")
            .MaximumLength(200)
            .WithMessage("Tên học phần không được vượt quá 200 ký tự.");

        RuleFor(c => c.Description)
            .MaximumLength(1000)
            .WithMessage("Mô tả không được vượt quá 1000 ký tự.");
    }

    private async Task<bool> BeUniqueCourseCode(string courseCode, CancellationToken token)
    {
        return await _courseRepository.IsCourseCodeUniqueAsync(courseCode, token);
    }
}
