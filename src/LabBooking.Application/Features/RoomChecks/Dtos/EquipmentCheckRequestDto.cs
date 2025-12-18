namespace LabBooking.Application.Features.RoomChecks.Dtos;

public record RoomCheckItemDto(
    Guid EquipmentId,
    bool IsOK,
    string? IssueDescription
);
