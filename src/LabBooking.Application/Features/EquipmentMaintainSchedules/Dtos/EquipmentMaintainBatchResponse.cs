namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

public class EquipmentMaintainBatchResponse
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = "NotYet";
    public int TotalEquipments { get; set; }

    public List<MaintainedEquipmentDto> Equipments { get; set; } = new();
}

public class MaintainedEquipmentDto
{
    public Guid MaintenanceId { get; set; }
    public Guid EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
