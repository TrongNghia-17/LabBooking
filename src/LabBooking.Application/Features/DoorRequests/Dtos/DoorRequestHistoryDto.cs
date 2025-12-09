namespace LabBooking.Application.Features.DoorRequests.Dtos;

public class DoorRequestHistoryDto
{
    public Guid Id { get; set; }
    public string LabRoomName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Open" / "Close"
    public string Status { get; set; } = string.Empty; // "Pending", "Accepted"...
    public DateTime RequestTime { get; set; }
    public DateTime? AcceptedTime { get; set; }

    // --- Thông tin NGƯỜI GỬI (Dành cho Bảo vệ xem) ---
    public string RequestedByName { get; set; } = string.Empty;
    public string RequestedByCode { get; set; } = string.Empty; // MSSV

    // --- Thông tin NGƯỜI XỬ LÝ (Dành cho Sinh viên xem) ---
    public string HandledByName { get; set; } = string.Empty;
    public string HandledByPhone { get; set; } = string.Empty;
}
