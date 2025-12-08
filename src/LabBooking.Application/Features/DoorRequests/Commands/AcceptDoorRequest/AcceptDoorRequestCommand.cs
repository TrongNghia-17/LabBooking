namespace LabBooking.Application.Features.DoorRequests.Commands.AcceptDoorRequest;

public record AcceptDoorRequestCommand(Guid RequestId) : IRequest<bool>;
