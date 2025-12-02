namespace LabBooking.Domain.Entities;

public class Incident
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    public Guid LabRoomId { get; set; }
    [ForeignKey(nameof(LabRoomId))]
    public LabRoom? LabRoom { get; set; }

    public Guid ReportedById { get; set; }
    [ForeignKey(nameof(ReportedById))]
    public User? ReportedBy { get; set; }

    public Guid? SlotId { get; set; }
    [ForeignKey(nameof(SlotId))]
    public Slot? Slot { get; set; }

    public Guid? EquipmentId { get; set; }
    [ForeignKey(nameof(EquipmentId))]
    public Equipment? Equipment { get; set; }

    public IncidentType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsResolved { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public LevelOfImportance ImportanceLevel { get; set; } = LevelOfImportance.Low;
}


