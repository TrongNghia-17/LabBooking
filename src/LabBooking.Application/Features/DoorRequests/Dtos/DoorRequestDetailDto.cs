namespace LabBooking.Application.Features.DoorRequests.Dtos;

public class DoorRequestDetailDto
{
    // Thông tin cơ bản của Request
    public Guid Id { get; set; }
    public string BookingCode { get; set; } = default!;

    public DateOnly RequestDate { get; set; }
    public Guid SlotId { get; set; }
    public string SlotLabel { get; set; } = string.Empty;
    public TimeOnly SlotStartTime { get; set; }
    public TimeOnly SlotEndTime { get; set; }

    public string Reason { get; set; } = default!;
    public string Status { get; set; } = default!; // Pending, Approved, Rejected
    public DateTime RequestTime { get; set; }


    // Thông tin xử lý (nếu đã duyệt)
    public DateTime? AcceptedTime { get; set; }
    public string? ManagerNote { get; set; }

    public string LabName { get; set; } = default!;


    // --- THÔNG TIN NGƯỜI GỬI (Dành riêng cho Manager) ---
    public string ContactName { get; set; } = default!;
    public string ContactEmail { get; set; } = default!;
    public string ContactPhoneNumber { get; set; } = default!;
    public string ContactRole { get; set; } = default!;
}
