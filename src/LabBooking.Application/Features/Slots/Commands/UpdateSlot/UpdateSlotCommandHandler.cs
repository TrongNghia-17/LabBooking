namespace LabBooking.Application.Features.Slots.Commands.UpdateSlot;

public class UpdateSlotCommandHandler(
    ILogger<UpdateSlotCommandHandler> logger,
    IMapper mapper,
    ISlotRepository slotRepository
    ) : IRequestHandler<UpdateSlotCommand>
{
    public async Task Handle(UpdateSlotCommand request, CancellationToken cancellationToken)
    {
        // 1. Lấy entity từ DB
        var slotToUpdate = await slotRepository.GetByIdAsync(request.Id, cancellationToken);

        if (slotToUpdate == null)
        {
            // Bạn có thể throw NotFoundException custom của bạn ở đây
            logger.LogWarning("Không tìm thấy Slot với ID: {SlotId}", request.Id);
            throw new KeyNotFoundException($"Không tìm thấy Slot với ID {request.Id}");
        }

        // 2. Map dữ liệu mới vào entity cũ
        mapper.Map(request, slotToUpdate);

        // 3. Lưu thay đổi
        await slotRepository.UpdateAsync(slotToUpdate, cancellationToken);

        logger.LogInformation("Đã cập nhật Slot {SlotId} thành công.", request.Id);
    }
}
