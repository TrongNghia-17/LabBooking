namespace LabBooking.Application.Features.DoorRequests.Dtos;

public class DoorRequestDetailDto
{
    // Thông tin cơ bản của Request
    public Guid Id { get; set; }
    public string BookingCode { get; set; } = default!;
    public string Reason { get; set; } = default!;
    public string Status { get; set; } = default!; // Pending, Approved, Rejected
    public DateTime RequestTime { get; set; }

    // Thông tin xử lý (nếu đã duyệt)
    public DateTime? AcceptedTime { get; set; }
    public string? ManagerNote { get; set; }

    // Thông tin phòng Lab (Lấy từ Booking)
    public string LabName { get; set; } = default!;

    // --- THÔNG TIN NGƯỜI GỬI (Dành riêng cho Manager) ---
    public string RequestedByName { get; set; } = default!;
    public string RequestedByEmail { get; set; } = default!;
    public string RequestedByPhoneNumber { get; set; } = default!;
}
