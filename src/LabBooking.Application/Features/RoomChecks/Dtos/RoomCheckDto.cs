namespace LabBooking.Application.Features.RoomChecks.Dtos;

public class RoomCheckDto
{
    public Guid Id { get; set; }
    public string LabRoomName { get; set; } = default!;
    public string SlotName { get; set; } = "Ngoài giờ"; // Hoặc tên Slot
    public string GuardName { get; set; } = default!; // Manager cần biết ai check

    public string Type { get; set; } = default!; // "CheckIn" / "CheckOut"
    public bool IsPassed { get; set; }
    public string? Note { get; set; }
    public DateTime CheckedAt { get; set; }
}
