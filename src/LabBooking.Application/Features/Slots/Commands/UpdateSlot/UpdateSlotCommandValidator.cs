namespace LabBooking.Application.Features.Slots.Commands.UpdateSlot;

public class UpdateSlotCommandValidator : AbstractValidator<UpdateSlotCommand>
{
    private readonly ISlotRepository _slotRepository;

    public UpdateSlotCommandValidator(ISlotRepository slotRepository)
    {
        _slotRepository = slotRepository;

        RuleFor(c => c.Id)
            .NotEmpty()
            .WithMessage("ID không hợp lệ.");

        RuleFor(c => c.SlotIndex)
            .InclusiveBetween(1, 4)
            .WithMessage("Slot phải nằm trong khoảng từ 1 đến 4.")
            .MustAsync(async (command, slotIndex, token) =>
                await BeUniqueSlotIndex(command.Id, slotIndex, token))
            .WithMessage("Slot (Ca) này đã tồn tại (trùng lặp với ca khác).");

        RuleFor(c => c.Label)
            .NotEmpty()
            .WithMessage("Tên Slot (Label) không được để trống.")
            .MaximumLength(50)
            .WithMessage("Tên Slot không được vượt quá 50 ký tự.");

        RuleFor(c => c.StartTime)
            .NotNull()
            .WithMessage("Thời gian bắt đầu không được để trống.");

        RuleFor(c => c.EndTime)
            .NotNull()
            .WithMessage("Thời gian kết thúc không được để trống.")
            .GreaterThan(c => c.StartTime)
            .WithMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
    }

    // Hàm check trùng lặp: Cho phép trùng nếu là chính bản thân record đang update
    private async Task<bool> BeUniqueSlotIndex(Guid id, int slotIndex, CancellationToken token)
    {
        return await _slotRepository.IsSlotIndexUniqueAsync(id, slotIndex, token);
    }
}
