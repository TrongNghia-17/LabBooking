namespace LabBooking.Application.Features.DoorRequests.Dtos;

public record UpdateDoorRequestStatusDto(
    DoorRequestStatus NewStatus,
    string? Note
);
