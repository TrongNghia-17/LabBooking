namespace LabBooking.Application.Features.Equipments.Dtos;

public record EquipmentResponse(
    Guid Id,
    string EquipmentName,
    string? Description,
    bool IsAvailable,
    Guid LabRoomId,
    EquipmentStatus Status
);

