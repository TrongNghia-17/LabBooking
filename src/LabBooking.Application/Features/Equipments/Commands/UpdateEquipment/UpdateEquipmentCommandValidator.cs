using FluentValidation;

namespace LabBooking.Application.Features.Equipments.Commands.UpdateEquipment;

/// <summary>
/// Defines validation rules for the <see cref="UpdateEquipmentCommand"/>.
/// </summary>
public class UpdateEquipmentCommandValidator : AbstractValidator<UpdateEquipmentCommand>
{
    public UpdateEquipmentCommandValidator()
    {
        RuleFor(c => c.EquipmentName)
            .NotEmpty()
            .WithMessage("Equipment Name is required.")
            .MaximumLength(100)
            .WithMessage("Equipment Name cannot be longer than 100 characters.");

        RuleFor(c => c.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot be longer than 500 characters.");

        RuleFor(c => c.LabRoomId)
            .NotEmpty()
            .WithMessage("Lab Room ID is required.");

        RuleFor(c => c.IsAvailable)
            .NotNull()
            .WithMessage("IsAvailable status is required.");

        RuleFor(c => c.Status)
            .NotEmpty()
            .WithMessage("'Status' is required.")
            .Must(BeValidEquipmentStatus)
            .When(c => !string.IsNullOrEmpty(c.Status))
            .WithMessage($"'Status' is invalid. Must be one of the following values: {GetValidStatuses()}");
    }

    /// <summary>
    /// Checks if the status string can be parsed to the EquipmentStatus enum.
    /// </summary>
    /// <param name="status">The status string to check.</param>
    /// <returns>True if parsing is successful, false otherwise.</returns>
    private bool BeValidEquipmentStatus(string? status)
    {
        return Enum.TryParse<EquipmentStatus>(status, true, out _);
    }

    /// <summary>
    /// Gets a comma-separated list of valid enum names for the error message.
    /// </summary>
    /// <returns>A string of valid status names.</returns>
    private string GetValidStatuses() => string.Join(", ", Enum.GetNames(typeof(EquipmentStatus)));
}
