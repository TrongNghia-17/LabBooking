namespace LabBooking.Application.Features.LabRooms.Dtos;

public record LabRoomResponse(
    Guid Id,
    string? LabName,
    string? Location,
    int? MaximumLimit,
    Guid? MainManagerId,
    Guid? CreatedById,
    DateTime CreatedDate,
    bool IsActive
);
