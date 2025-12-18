namespace LabBooking.Application.Features.RoomChecks.Commands.CreateRoomCheck;

public record CreateRoomCheckCommand(
    Guid LabRoomId,
    Guid? SlotId,   // Bắt buộc chọn Slot để biết check ca nào
    CheckType Type, // In hoặc Out
    bool IsPassed,  // True/False
    string? Note
) : IRequest<Guid>;