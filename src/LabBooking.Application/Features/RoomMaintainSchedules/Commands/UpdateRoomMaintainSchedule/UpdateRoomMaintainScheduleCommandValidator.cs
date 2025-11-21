namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.UpdateRoomMaintainSchedule;

public class UpdateRoomMaintainScheduleCommandValidator : AbstractValidator<UpdateRoomMaintainScheduleCommand>
{
    private readonly ILabRoomRepository _labRoomRepository;

    public UpdateRoomMaintainScheduleCommandValidator(ILabRoomRepository labRoomRepository)
    {
        _labRoomRepository = labRoomRepository;

        // --- LabRoomId Rules (Giống Create) ---
        RuleFor(c => c.LabRoomId)
            .NotEmpty().WithMessage("LabRoomId is required.")
            .MustAsync(LabRoomMustExist)
            .WithMessage("The specified LabRoom was not found.");

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
        RuleFor(c => c.RoomMaintainStatus)
            .NotNull().WithMessage("Room Maintain Status is required.") // Yêu cầu phải có status
            .IsInEnum().WithMessage("Invalid status value."); // Phải là giá trị hợp lệ (Done hoặc NotYet)
    }

    private async Task<bool> LabRoomMustExist(Guid id, CancellationToken token)
    {
        // Giả định ILabRoomRepository đã có ExistsAsync
        return await _labRoomRepository.ExistsAsync(id, token);
    }
}
