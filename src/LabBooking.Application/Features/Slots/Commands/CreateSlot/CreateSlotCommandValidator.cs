namespace LabBooking.Application.Features.Slots.Commands.CreateSlot;

public class CreateSlotCommandValidator : AbstractValidator<CreateSlotCommand>
{
    private readonly ISlotRepository _slotRepository;

    public CreateSlotCommandValidator(ISlotRepository slotRepository)
    {
        _slotRepository = slotRepository;

        RuleFor(c => c.SlotIndex)
            .InclusiveBetween(1, 4)
            .WithMessage("Slot phải nằm trong khoảng từ 1 đến 4.")
            .MustAsync(BeUniqueSlotIndex)
            .WithMessage("Slot (Ca) này đã tồn tại trong hệ thống.");

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

    private async Task<bool> BeUniqueSlotIndex(int slotIndex, CancellationToken token)
    {
        // Hàm này sẽ được implement trong Repository bên dưới
        return await _slotRepository.IsSlotIndexUniqueAsync(slotIndex, token);
    }
}
