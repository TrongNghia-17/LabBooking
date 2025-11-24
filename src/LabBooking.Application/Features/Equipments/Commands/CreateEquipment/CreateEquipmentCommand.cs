namespace LabBooking.Application.Features.Equipments.Commands.CreateEquipment;

/// <summary>
/// Represents the command to create a new equipment.
/// </summary>
/// <param name="EquipmentName">The name of the equipment.</param>
/// <param name="Description">An optional description for the equipment.</param>
/// <param name="LabRoomId">The unique identifier of the lab room where the equipment is located.</param>
/// <param name="Status">The initial status of the equipment (e.g., "Available").</param>
public record CreateEquipmentCommand(
    string EquipmentName,
    string? Description,
    Guid LabRoomId
) : IRequest<Guid>;
