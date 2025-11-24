namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;

public class CreateEquipmentMaintainScheduleCommandValidator : AbstractValidator<CreateEquipmentMaintainScheduleCommand>
{
    // Giả định bạn có một repository cho Equipment
    private readonly IEquipmentRepository _equipmentRepository;

    public CreateEquipmentMaintainScheduleCommandValidator(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;

        // --- EquipmentId Rules ---
        RuleFor(c => c.EquipmentId)
            .NotEmpty().WithMessage("EquipmentId is required.")
            .MustAsync(EquipmentMustExist) // Giả định IEquipmentRepository có ExistsAsync
            .WithMessage("The specified Equipment was not found.");

        // --- Date Logic (Tương tự RoomMaintainSchedule) ---
        RuleFor(c => c.EndTime)
            .GreaterThan(c => c.StartTime)
            .WithMessage("End Time must be after Start Time.")
            .When(c => c.StartTime.HasValue && c.EndTime.HasValue);

        // --- NumberOfSlot Logic ---
        RuleFor(c => c.NumberOfSlot)
            .GreaterThan(0)
            .When(c => c.NumberOfSlot.HasValue)
            .WithMessage("Number of Slots must be greater than 0.");

        // --- Description Logic ---
        RuleFor(c => c.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }

    /// <summary>
    /// Kiểm tra Equipment có tồn tại hay không
    /// </summary>
    private async Task<bool> EquipmentMustExist(Guid id, CancellationToken token)
    {
        // Giả định IEquipmentRepository có phương thức ExistsAsync
        return await _equipmentRepository.ExistsAsync(id, token);
    }
}
