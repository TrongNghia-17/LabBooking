namespace LabBooking.Domain.Entities;

public enum IncidentType
{
    Fire,
    PowerOutage,
    EquipmentFailure,
    SecurityIssue,
    Opened,
    Closed,
    Other
}

public class Incident
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    public Guid LabRoomId { get; set; }
    [ForeignKey(nameof(LabRoomId))]
    public LabRoom? LabRoom { get; set; }

    public Guid ReportedById { get; set; }
    [ForeignKey(nameof(ReportedById))]
    public User? ReportedBy { get; set; }

    public Guid BookingId { get; set; }
    [ForeignKey(nameof(BookingId))]
    public Booking? Booking { get; set; }

    public IncidentType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsResolved { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
