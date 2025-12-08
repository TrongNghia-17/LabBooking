namespace LabBooking.Domain.Entities;

public class IncidentEquipment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    // Link ngược về Incident cha
    public Guid IncidentId { get; set; }
    [ForeignKey(nameof(IncidentId))]
    public Incident Incident { get; set; } = null!;

    // Link sang Thiết bị bị hỏng
    public Guid EquipmentId { get; set; }
    [ForeignKey(nameof(EquipmentId))]
    public Equipment Equipment { get; set; } = null!;
}