using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.UpdateEquipmentMaintainSchedule;

public record UpdateEquipmentMaintainScheduleCommand() : IRequest<Unit>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Description { get; set; }
}
