using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetUnmaintainedLabRooms;

/// <summary>
/// Query để lấy tất cả phòng Lab có lịch bảo trì nhưng chưa hoàn thành (NotYet).
/// </summary>
public record GetUnmaintainedLabRoomsQuery() : IRequest<IEnumerable<LabRoomResponse>>;
