namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentCommandValidator : AbstractValidator<CreateIncidentCommand>
{
    private readonly ILabRoomRepository _labRoomRepository;
    private readonly ISlotRepository _slotRepository; // Giả sử bạn đã có repo này từ task trước

    public CreateIncidentCommandValidator(
        ILabRoomRepository labRoomRepository,
        ISlotRepository slotRepository)
    {
        _labRoomRepository = labRoomRepository;
        _slotRepository = slotRepository;

        RuleFor(c => c.LabRoomId)
            .NotEmpty().WithMessage("Vui lòng chọn phòng Lab.")
            .MustAsync(LabRoomMustExist).WithMessage("Phòng Lab không tồn tại.");

        RuleFor(c => c.SlotId)
            .NotEmpty().WithMessage("Vui lòng chọn Slot (Ca).")
            .MustAsync(SlotMustExist).WithMessage("Slot không tồn tại.");

        RuleFor(c => c.Type)
            .IsInEnum().WithMessage("Loại sự cố không hợp lệ.");

        RuleFor(c => c.ImportanceLevel)
            .IsInEnum().WithMessage("Mức độ nghiêm trọng không hợp lệ.");

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("Mô tả sự cố không được để trống.")
            .MaximumLength(1000).WithMessage("Mô tả không được quá 1000 ký tự.");
    }

    private async Task<bool> LabRoomMustExist(Guid labRoomId, CancellationToken token)
    {
        return await _labRoomRepository.ExistsAsync(labRoomId, token);
    }

    private async Task<bool> SlotMustExist(Guid slotId, CancellationToken token)
    {
        // Giả sử ISlotRepository có hàm ExistsAsync, nếu chưa có bạn dùng GetById != null
        return await _slotRepository.GetByIdAsync(slotId, token) != null;
    }
}
