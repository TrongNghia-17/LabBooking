namespace LabBooking.Application.Features.DoorRequests.Dtos;

public class BookingForDoorOpenDto
{
    public Guid BookingId { get; set; }
    public Guid LabRoomId { get; set; }
    public string LabRoomName { get; set; } = string.Empty;
    public string SlotName { get; set; } = string.Empty; // Ví dụ: Ca 1
    public TimeOnly StartTime { get; set; } // 07:00
    public TimeOnly EndTime { get; set; }   // 09:00
    public DateOnly Date { get; set; }
}
