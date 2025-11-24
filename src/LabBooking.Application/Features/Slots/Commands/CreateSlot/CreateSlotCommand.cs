namespace LabBooking.Application.Features.Slots.Commands.CreateSlot;

/// <summary>
/// Record chứa dữ liệu đầu vào để tạo một Slot mới.
/// </summary>
/// <param name="StartTime">Thời gian bắt đầu.</param>
/// <param name="EndTime">Thời gian kết thúc.</param>
/// <param name="SlotIndex">Chỉ số của slot (1-4).</param>
/// <param name="Label">Nhãn hiển thị (VD: Ca sáng, Ca chiều).</param>
public record CreateSlotCommand(
    TimeOnly StartTime,
    TimeOnly EndTime,
    int SlotIndex,
    string Label
) : IRequest<Guid>;
