using LabBooking.Domain.Enums;

namespace LabBooking.Domain.Entities;

public class EquipmentMaintenance
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    public Guid EquipmentMaintainScheduleId { get; set; }

    [ForeignKey(nameof(EquipmentMaintainScheduleId))]
    public EquipmentMaintainSchedule? Schedule { get; set; }

    public Guid EquipmentId { get; set; }

    [ForeignKey(nameof(EquipmentId))]
    public Equipment? Equipment { get; set; }

    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.NotYet;
    public string? ResultNote { get; set; }
}
