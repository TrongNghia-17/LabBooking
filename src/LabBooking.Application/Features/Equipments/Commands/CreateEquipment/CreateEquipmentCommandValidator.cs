namespace LabBooking.Application.Features.Equipments.Commands.CreateEquipment;

public class CreateEquipmentCommandValidator : AbstractValidator<CreateEquipmentCommand>
{
    public CreateEquipmentCommandValidator()
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

        //RuleFor(c => c.Status)
        //    .NotEmpty()
        //    .WithMessage("'Status' is required.")
        //    .Must(BeValidEquipmentStatus)
        //    .When(c => !string.IsNullOrEmpty(c.Status))
        //    .WithMessage($"'Status' is not valid. Must be one of: {GetValidStatuses()}");
    }

    /// <summary>
    /// Validates if the provided status string can be parsed into the EquipmentStatus enum.
    /// </summary>
    /// <param name="status">The status string to validate.</param>
    /// <returns>True if the string is a valid EquipmentStatus, false otherwise.</returns>
    /// <remarks>
    /// The check is case-insensitive (e.g., "available" or "Available" are both valid).
    /// </remarks>
    private bool BeValidEquipmentStatus(string? status)
    {
        return Enum.TryParse<EquipmentStatus>(status, true, out _);
    }

    /// <summary>
    /// Gets a comma-separated list of valid EquipmentStatus names for the error message.
    /// </summary>
    /// <returns>A string of valid status names.</returns>
    private string GetValidStatuses() => string.Join(", ", Enum.GetNames(typeof(EquipmentStatus)));
}
