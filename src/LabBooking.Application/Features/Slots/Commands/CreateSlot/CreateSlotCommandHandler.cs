namespace LabBooking.Application.Features.Slots.Commands.CreateSlot;

public class CreateSlotCommandHandler(
    ILogger<CreateSlotCommandHandler> logger,
    IMapper mapper,
    ISlotRepository slotRepository
    ) : IRequestHandler<CreateSlotCommand, Guid>
{
    public async Task<Guid> Handle(CreateSlotCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang tạo Slot mới: {Label} ({SlotIndex})", request.Label, request.SlotIndex);

        // Map từ Command sang Entity
        var slot = mapper.Map<Slot>(request);

        // Gọi Repository để lưu
        var slotId = await slotRepository.Create(slot, cancellationToken);

        logger.LogInformation("Đã tạo Slot thành công với ID: {SlotId}", slotId);

        return slotId;
    }
}
