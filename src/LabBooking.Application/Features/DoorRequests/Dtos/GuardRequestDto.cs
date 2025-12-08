namespace LabBooking.Application.Features.DoorRequests.Dtos;

public class GuardRequestDto
{
    public Guid RequestId { get; set; }
    public string LabRoomName { get; set; } = string.Empty;
    public DateTime RequestTime { get; set; }

    // --- THÔNG TIN ĐỂ BẢO VỆ CHECK ---
    public string Name { get; set; } = string.Empty; // Họ tên
    public string Email { get; set; } = string.Empty; // MSSV (Lấy từ UserName)
    public string PhoneNumber { get; set; } = string.Empty; // SĐT liên hệ
}
