namespace LabBooking.Application.Features.Equipments.Dtos;

/// <summary>
/// Represents the data transfer object for an equipment.
/// </summary>
public record EquipmentResponse(
    Guid Id,
    string EquipmentName,
    string? Description,
    bool IsAvailable,
    Guid LabRoomId,
    EquipmentStatus Status,
    Guid? EquipmentCategoryId,
    string? EquipmentCategoryName
);

