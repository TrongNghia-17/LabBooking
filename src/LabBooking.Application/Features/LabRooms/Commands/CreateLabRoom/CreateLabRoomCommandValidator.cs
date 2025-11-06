namespace LabBooking.Application.Features.LabRooms.Commands.CreateLabRoom;

public class CreateLabRoomCommandValidator : AbstractValidator<CreateLabRoomCommand>
{
    public CreateLabRoomCommandValidator()
    {
        RuleFor(c => c.LabName)
            .NotEmpty()
            .WithMessage("Lab Name is required.")
            .MaximumLength(100)
            .WithMessage("Lab Name cannot be longer than 100 characters.");

        RuleFor(c => c.Location)
            .MaximumLength(200)
            .WithMessage("Location cannot be longer than 200 characters.");

        RuleFor(c => c.MaximumLimit)
            .GreaterThan(0)
            .When(c => c.MaximumLimit.HasValue)
            .WithMessage("Maximum Limit must be greater than 0.");

        RuleFor(c => c.CreatedById)
            .NotEmpty()
            .WithMessage("Created By ID cannot be empty.");
    }
}

