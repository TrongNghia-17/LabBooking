namespace LabBooking.Application.Features.Supports.Commands.UpdateSupport;

public class UpdateSupportCommandValidator : AbstractValidator<UpdateSupportCommand>
{
    public UpdateSupportCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(100)
            .WithMessage("Title cannot be longer than 100 characters.");

        RuleFor(c => c.Content)
            .NotEmpty()
            .WithMessage("Content is required.")
            .MaximumLength(1000)
            .WithMessage("Content cannot be longer than 1000 characters.");

        RuleFor(c => c.Answer)
            .MaximumLength(2000)
            .WithMessage("Answer cannot be longer than 2000 characters.");
    }
}
