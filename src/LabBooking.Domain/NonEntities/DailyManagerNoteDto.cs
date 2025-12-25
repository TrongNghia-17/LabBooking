namespace LabBooking.Domain.NonEntities;

public class DailyManagerNoteDto
{
    public Guid RequestId { get; set; }
    public string LabName { get; set; } = string.Empty;     // Phòng nào?
    public string SlotTime { get; set; } = string.Empty;    // Khung giờ nào?
    public string RequesterName { get; set; } = string.Empty; // Ai đến?
    public string ManagerNote { get; set; } = string.Empty; // Dặn dò gì? (Quan trọng nhất)
    public string BookingCode { get; set; } = string.Empty;
}
