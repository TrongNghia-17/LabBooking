namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.UpdateEquipmentMaintainSchedule;

public class UpdateEquipmentMaintainScheduleCommandValidator : AbstractValidator<UpdateEquipmentMaintainScheduleCommand>
{
    private readonly IEquipmentRepository _equipmentRepository;

    public UpdateEquipmentMaintainScheduleCommandValidator(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;

        // --- EquipmentId Rules (Giống Create) ---
        RuleFor(c => c.EquipmentId)
            .NotEmpty().WithMessage("EquipmentId is required.")
            .MustAsync(EquipmentMustExist) // Giả định IEquipmentRepository đã có ExistsAsync
            .WithMessage("The specified Equipment was not found.");

        // --- Date Logic (Giống Create) ---
        RuleFor(c => c.EndTime)
            .GreaterThan(c => c.StartTime)
            .WithMessage("End Time must be after Start Time.")
            .When(c => c.StartTime.HasValue && c.EndTime.HasValue);

        // --- NumberOfSlot Logic (Giống Create) ---
        RuleFor(c => c.NumberOfSlot)
            .GreaterThan(0)
            .When(c => c.NumberOfSlot.HasValue)
            .WithMessage("Number of Slots must be greater than 0.");

        // --- Description Logic (Giống Create) ---
        RuleFor(c => c.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        // --- Status Logic (Rule mới) ---
        RuleFor(c => c.EquimentpMaintainStatus)
            .NotNull().WithMessage("Equipment Maintain Status is required.")
            .IsInEnum().WithMessage("Invalid status value."); // Phải là giá trị hợp lệ (Done hoặc NotYet)
    }

    private async Task<bool> EquipmentMustExist(Guid id, CancellationToken token)
    {
        // Sử dụng phương thức ExistsAsync đã thêm ở bước trước
        return await _equipmentRepository.ExistsAsync(id, token);
    }
}
