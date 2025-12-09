namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.CreateRoomMaintainSchedule;

public class CreateRoomMaintainScheduleCommandValidator : AbstractValidator<CreateRoomMaintainScheduleCommand>
{
    private readonly ILabRoomRepository _labRoomRepository;

    public CreateRoomMaintainScheduleCommandValidator(ILabRoomRepository labRoomRepository)
    {
        _labRoomRepository = labRoomRepository;

        // --- LabRoomId Rules ---
        RuleFor(c => c.LabRoomId)
            .NotEmpty().WithMessage("LabRoomId is required.")
            .MustAsync(LabRoomMustExist)
            .WithMessage("The specified LabRoom was not found.");

        // --- Start Time Logic (MỚI THÊM) ---
        RuleFor(c => c.StartTime)
            .NotEmpty().WithMessage("Start Time is required.") // Bắt buộc nhập
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Thời gian bắt đầu bảo trì phải sau thời điểm hiện tại.")
            .When(c => c.StartTime.HasValue);

        // --- Date Logic (Tương tự UsagePolicy) ---
        RuleFor(c => c.EndTime)
            .GreaterThan(c => c.StartTime)
            .WithMessage("End Time must be after Start Time.");
        //.When(c => c.StartTime.HasValue && c.EndTime.HasValue);

        // --- Description Logic ---
        RuleFor(c => c.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }

    /// <summary>
    /// Kiểm tra LabRoom có tồn tại hay không
    /// </summary>
    private async Task<bool> LabRoomMustExist(Guid id, CancellationToken token)
    {
        // Sử dụng phương thức ExistsAsync chúng ta đã thêm vào ILabRoomRepository
        return await _labRoomRepository.ExistsAsync(id, token);
    }
}
