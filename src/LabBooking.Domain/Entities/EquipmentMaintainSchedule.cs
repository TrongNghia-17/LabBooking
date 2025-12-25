namespace LabBooking.Domain.Entities;

public class EquipmentMaintainSchedule
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();
    public Guid? CreatedBy { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.NotYet;


    public ICollection<EquipmentMaintenance> Details { get; set; } = new List<EquipmentMaintenance>();
}
