namespace LabBooking.Application.Features.Supports.Commands.UpdateSupport;

/// <summary>
/// Defines the validation rules for the <see cref="UpdateSupportCommand"/>.
/// </summary>
public class UpdateSupportCommandValidator : AbstractValidator<UpdateSupportCommand>
{
    public UpdateSupportCommandValidator()
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

        RuleFor(c => c.Answer)
            .MaximumLength(2000)
            .WithMessage("The answer must not exceed 2000 characters.");
    }
}
