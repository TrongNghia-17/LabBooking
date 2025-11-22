namespace LabBooking.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    private readonly ICourseRepository _courseRepository;

    public UpdateCourseCommandValidator(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;

        RuleFor(c => c.Id)
            .NotEmpty()
            .WithMessage("ID không hợp lệ.");

        RuleFor(c => c.CourseCode)
            .NotEmpty()
            .WithMessage("Mã học phần không được để trống.")
            .MaximumLength(20)
            .WithMessage("Mã học phần không được vượt quá 20 ký tự.")
            .MustAsync(async (command, code, token) =>
                await BeUniqueCourseCode(command.Id, code, token))
            .WithMessage("Mã học phần này đã tồn tại (trùng với học phần khác).");

        RuleFor(c => c.CourseName)
            .NotEmpty()
            .WithMessage("Tên học phần không được để trống.")
            .MaximumLength(200)
            .WithMessage("Tên học phần không được vượt quá 200 ký tự.");

        RuleFor(c => c.Description)
            .MaximumLength(1000)
            .WithMessage("Mô tả không được vượt quá 1000 ký tự.");
    }

    // Hàm check trùng lặp: Cho phép trùng nếu là chính bản thân record đang update
    private async Task<bool> BeUniqueCourseCode(Guid id, string courseCode, CancellationToken token)
    {
        return await _courseRepository.IsCourseCodeUniqueAsync(id, courseCode, token);
    }
}
