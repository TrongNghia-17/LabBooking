namespace LabBooking.Application.Features.RoomChecks.Commands.DeleteRoomCheck;

public record DeleteRoomCheckCommand(Guid Id) : IRequest<Unit>;

