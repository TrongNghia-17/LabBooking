namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.UpdateRoomMaintainSchedule;

public class UpdateRoomMaintainScheduleCommandValidator : AbstractValidator<UpdateRoomMaintainScheduleCommand>
{
    private readonly ILabRoomRepository _labRoomRepository;

    public UpdateRoomMaintainScheduleCommandValidator(ILabRoomRepository labRoomRepository)
    {
        _labRoomRepository = labRoomRepository;

        // --- Date Logic (Giống Create) ---
        RuleFor(c => c.EndTime)
            .GreaterThan(c => c.StartTime)
            .WithMessage("End Time must be after Start Time.")
            .When(c => c.StartTime.HasValue && c.EndTime.HasValue);

        // --- Description Logic (Giống Create) ---
        RuleFor(c => c.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }

    private async Task<bool> LabRoomMustExist(Guid id, CancellationToken token)
    {
        // Giả định ILabRoomRepository đã có ExistsAsync
        return await _labRoomRepository.ExistsAsync(id, token);
    }
}
