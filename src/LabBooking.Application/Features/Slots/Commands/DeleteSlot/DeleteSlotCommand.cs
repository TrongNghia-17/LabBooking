namespace LabBooking.Application.Features.Slots.Commands.DeleteSlot;

// Command chỉ cần chứa ID của Slot muốn xóa
public record DeleteSlotCommand(Guid Id) : IRequest;
