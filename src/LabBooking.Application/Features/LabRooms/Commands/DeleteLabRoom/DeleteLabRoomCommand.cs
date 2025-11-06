namespace LabBooking.Application.Features.LabRooms.Commands.DeleteLabRoom;

/// <summary>
/// Command để xử lý logic xóa một LabRoom.
/// Sử dụng IRequest<Unit> vì không cần trả về dữ liệu gì sau khi xóa.
/// </summary>
public record DeleteLabRoomCommand(Guid Id) : IRequest<Unit>;
