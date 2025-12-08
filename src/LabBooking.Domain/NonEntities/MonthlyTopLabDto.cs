namespace LabBooking.Application.Features.LabRooms.Dtos;

public class MonthlyTopLabDto
{
    public string MonthYear { get; set; } = string.Empty; // Ví dụ: "12/2024"
    public string LabName { get; set; } = string.Empty;   // Tên phòng hot nhất
    public int TotalBookings { get; set; }                // Số lần đặt
}
