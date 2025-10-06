namespace LabBooking.Application.Features.DoorRequests.Dtos;

public record DoorRequestsResponse(
    Guid Id,
    Guid LabRoomId,
    Guid RequestedById,
    DateTime RequestTime,
    DoorRequestStatus Status,
    Guid? HandledById
 );
