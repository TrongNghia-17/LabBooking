namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.CreateRoomMaintainSchedule;

/// <summary>
/// Command chứa dữ liệu để tạo một lịch bảo trì phòng lab mới.
/// </summary>
public record CreateRoomMaintainScheduleCommand(
    Guid LabRoomId,
    DateTime? StartTime,
    DateTime? EndTime,
    string? Description
) : IRequest<Guid>;
