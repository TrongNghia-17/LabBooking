namespace LabBooking.Domain.Entities;

public enum CheckType
{
    CheckIn,   // Đầu ca/Đầu giờ
    CheckOut,  // Cuối ca/Cuối giờ
    Periodic   // Kiểm tra định kỳ
}

public class RoomCheck
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    // Ai là người kiểm tra? (Bảo vệ)
    public Guid GuardId { get; set; }
    [ForeignKey(nameof(GuardId))]
    public User? Guard { get; set; }

    // Kiểm tra phòng nào?
    public Guid LabRoomId { get; set; }
    [ForeignKey(nameof(LabRoomId))]
    public LabRoom? LabRoom { get; set; }

    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public CheckType Type { get; set; }
    public string? Note { get; set; } // Ghi chú chung (VD: Phòng bẩn, cửa chưa khóa...)

    // Chi tiết từng thiết bị
    public ICollection<EquipmentCheckResult> Details { get; set; } = new List<EquipmentCheckResult>();
}
