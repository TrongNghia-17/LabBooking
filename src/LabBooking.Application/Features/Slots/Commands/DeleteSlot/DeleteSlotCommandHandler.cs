namespace LabBooking.Application.Features.Slots.Commands.DeleteSlot;

public class DeleteSlotCommandHandler(
    ILogger<DeleteSlotCommandHandler> logger,
    ISlotRepository slotRepository
    ) : IRequestHandler<DeleteSlotCommand>
{
    public async Task Handle(DeleteSlotCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra Slot có tồn tại không
        var slotToDelete = await slotRepository.GetByIdAsync(request.Id, cancellationToken);

        if (slotToDelete == null)
        {
            logger.LogWarning("Cố gắng xóa Slot không tồn tại. ID: {SlotId}", request.Id);
            throw new KeyNotFoundException($"Không tìm thấy Slot với ID: {request.Id}");
        }

        // 2. (Tùy chọn) Kiểm tra ràng buộc dữ liệu
        // Nếu sau này có bảng Booking, bạn cần kiểm tra xem Slot này có đang được book không
        // trước khi cho phép xóa. Ví dụ:
        // if (await slotRepository.HasBookings(request.Id)) throw new ...

        // 3. Thực hiện xóa
        await slotRepository.DeleteAsync(slotToDelete, cancellationToken);

        logger.LogInformation("Đã xóa thành công Slot có ID: {SlotId}", request.Id);
    }
}
