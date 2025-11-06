namespace LabBooking.Application.Features.Supports.Commands.CreateSupport;

public class CreateSupportCommandValidator : AbstractValidator<CreateSupportCommand>
{
    public CreateSupportCommandValidator()
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

        RuleFor(c => c.CreatedById)
            .NotEmpty()
            .WithMessage("Created By ID cannot be empty.");
    }
}
