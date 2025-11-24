using LabBooking.Application.Features.RoomMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Queries.GetRoomMaintainScheduleById;

/// <summary>
/// Record chứa ID để truy vấn một RoomMaintainSchedule.
/// </summary>
public record GetRoomMaintainScheduleByIdQuery(Guid Id) : IRequest<RoomMaintainScheduleResponse>;
