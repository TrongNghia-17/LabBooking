namespace LabBooking.Application.Features.DoorRequests.Commands.CreateDoorRequest;

public record CreateDoorRequestCommand(
    string BookingCode,
    string Reason
) : IRequest<Guid>;
