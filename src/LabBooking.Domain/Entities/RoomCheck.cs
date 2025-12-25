using LabBooking.Domain.Common;

namespace LabBooking.Domain.Entities;

public enum CheckType
{
    CheckIn,   // Đầu ca
    CheckOut   // Cuối ca
}

public class RoomCheck : ISoftDelete
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    // Ai kiểm tra
    public Guid GuardId { get; set; }
    [ForeignKey(nameof(GuardId))]
    public User? Guard { get; set; }

    // Phòng nào
    public Guid LabRoomId { get; set; }
    [ForeignKey(nameof(LabRoomId))]
    public LabRoom? LabRoom { get; set; }

    // Ca nào (Quan trọng)
    public Guid? SlotId { get; set; }
    [ForeignKey(nameof(SlotId))]
    public Slot? Slot { get; set; }

    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public CheckType Type { get; set; }

    // Kết quả: True = Tốt, False = Có vấn đề (Cần tạo Incident)
    public bool IsPassed { get; set; }
    public string? Note { get; set; } // Ghi chú: "Phòng sạch", "Mất chuột"...
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}