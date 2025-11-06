namespace LabBooking.Domain.Entities;

public class Equipment
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();
    [Required]
    public string EquipmentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsAvailable { get; set; } = true;
    public Guid LabRoomId { get; set; }
    [ForeignKey(nameof(LabRoomId))]
    public LabRoom? LabRoom { get; set; }
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;
}
public enum EquipmentStatus
{
    Maintain,
    Available,
    Broken,
    Other
}
