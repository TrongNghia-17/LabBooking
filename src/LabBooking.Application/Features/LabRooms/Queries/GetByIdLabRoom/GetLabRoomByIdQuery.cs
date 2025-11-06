using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetByIdLabRoom;

public record GetLabRoomByIdQuery(Guid Id) : IRequest<LabRoomResponse>;

