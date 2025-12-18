namespace LabBooking.Domain.Entities;

public class EquipmentCheckResult
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    public Guid RoomCheckId { get; set; }
    [ForeignKey(nameof(RoomCheckId))]
    public RoomCheck? RoomCheck { get; set; }

    public Guid EquipmentId { get; set; }
    [ForeignKey(nameof(EquipmentId))]
    public Equipment? Equipment { get; set; }

    public bool IsOK { get; set; } // True: Tốt, False: Hỏng/Mất
    public string? IssueDescription { get; set; } // Mô tả lỗi (nếu IsOK = false)

    // 🔥 LIÊN KẾT TỰ ĐỘNG VỚI INCIDENT
    public Guid? IncidentId { get; set; }
    [ForeignKey(nameof(IncidentId))]
    public Incident? Incident { get; set; }
}
