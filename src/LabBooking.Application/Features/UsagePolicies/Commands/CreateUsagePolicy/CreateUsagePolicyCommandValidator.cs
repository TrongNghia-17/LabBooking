namespace LabBooking.Application.Features.UsagePolicies.Commands.CreateUsagePolicy;

/// <summary>
/// Defines validation rules for the <see cref="CreateUsagePolicyCommand"/>.
/// </summary>
public class CreateUsagePolicyCommandValidator : AbstractValidator<CreateUsagePolicyCommand>
{
    public CreateUsagePolicyCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(c => c.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        // --- Conditional Logic for LabRoomId ---

        RuleFor(c => c.LabRoomId)
            .NotEmpty()
            .WithMessage("LabRoomId is required when 'ForAllLabRooms' is false.")
            // Only apply this rule if ForAllLabRooms is false
            .When(c => !c.ForAllLabRooms);

        RuleFor(c => c.LabRoomId)
            .Empty()
            .WithMessage("LabRoomId must be null when 'ForAllLabRooms' is true.")
            // Only apply this rule if ForAllLabRooms is true
            .When(c => c.ForAllLabRooms);

        // --- Date Logic ---

        RuleFor(c => c.ExpirationDate)
            .GreaterThan(c => c.EffectiveFrom)
            .WithMessage("Expiration Date must be after the Effective From date.")
            // Only apply this rule if both dates are provided
            .When(c => c.ExpirationDate.HasValue && c.EffectiveFrom.HasValue);
    }
}
