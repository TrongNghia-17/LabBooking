using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.Equipments.Commands.UpdateEquipment;

/// <summary>
/// Represents the command to update an existing equipment's details.
/// </summary>
public record UpdateEquipmentCommand() : IRequest<Unit>
{
    /// <summary>
    /// The unique identifier of the equipment to update.
    /// This is typically set from the route and ignored in the request body.
    /// </summary>
    [JsonIgnore]
    public Guid Id { get; set; }

    public string EquipmentName { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsAvailable { get; set; }
    public Guid LabRoomId { get; set; }
    public string Status { get; set; } = default!;
}
