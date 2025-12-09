namespace LabBooking.Application.Features.DoorRequests.Commands.CancelDoorRequest;

public record CancelDoorRequestCommand(Guid RequestId) : IRequest<bool>;
