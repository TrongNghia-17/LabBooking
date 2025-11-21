namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.DeleteRoomMaintainSchedule;

/// <summary>
/// Command để xử lý logic xóa một RoomMaintainSchedule.
/// </summary>
public record DeleteRoomMaintainScheduleCommand(Guid Id) : IRequest<Unit>;
