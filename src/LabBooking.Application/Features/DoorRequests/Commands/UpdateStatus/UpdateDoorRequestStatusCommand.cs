namespace LabBooking.Application.Features.DoorRequests.Commands.UpdateStatus;

public record UpdateDoorRequestStatusCommand(
    Guid Id,
    DoorRequestStatus NewStatus,
    string Note
) : IRequest<Unit>;
