using LabBooking.Domain.Common;

namespace LabBooking.Domain.Entities;

public class Incident : ISoftDelete
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    public Guid LabRoomId { get; set; }
    [ForeignKey(nameof(LabRoomId))]
    public LabRoom? LabRoom { get; set; }

    public Guid ReportedById { get; set; }
    [ForeignKey(nameof(ReportedById))]
    public User? ReportedBy { get; set; }

    public Guid? RoomCheckId { get; set; }
    [ForeignKey(nameof(RoomCheckId))]
    public RoomCheck? RoomCheck { get; set; }

    public ICollection<IncidentEquipment> IncidentEquipments { get; set; } = new List<IncidentEquipment>();

    public IncidentType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsResolved { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public LevelOfImportance ImportanceLevel { get; set; } = LevelOfImportance.Low;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}


