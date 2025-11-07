using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.Equipments.Commands.UpdateEquipment;

public record UpdateEquipmentCommand() : IRequest<Unit>
{
    [JsonIgnore]
    public Guid Id { get; set; }

    public string EquipmentName { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsAvailable { get; set; }
    public Guid LabRoomId { get; set; }
    public string Status { get; set; } = default!;
}
