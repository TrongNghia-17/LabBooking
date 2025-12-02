namespace LabBooking.Application.Features.DoorRequests.Commands.Create;

public record CreateDoorRequestCommand(
    Guid LabRoomId,
    DoorRequestType Type // Open hoặc Close
) : IRequest<Guid>;
