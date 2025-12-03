namespace LabBooking.Application.Features.DoorRequests.Commands.Create;

public record CreateDoorRequestCommand(
    Guid LabRoomId
) : IRequest<Guid>;
