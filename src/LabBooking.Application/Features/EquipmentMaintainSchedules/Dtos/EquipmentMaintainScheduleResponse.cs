namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

public class EquipmentMaintainScheduleResponse
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int EquipmentCount { get; set; }
    public List<string> EquipmentNames { get; set; } = new();
}
