namespace LabBooking.Application.Features.Supports.Commands.CreateSupport;

/// <summary>
/// Defines the validation rules for the <see cref="CreateSupportCommand"/>.
/// </summary>
public class CreateSupportCommandValidator : AbstractValidator<CreateSupportCommand>
{
    public CreateSupportCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty()
            .WithMessage("A title is required for the support ticket.")
            .MaximumLength(100)
            .WithMessage("The title must not exceed 100 characters.");

        RuleFor(c => c.Content)
            .NotEmpty()
            .WithMessage("Content is required for the support ticket.")
            .MaximumLength(1000)
            .WithMessage("The content must not exceed 1000 characters.");
    }
}
