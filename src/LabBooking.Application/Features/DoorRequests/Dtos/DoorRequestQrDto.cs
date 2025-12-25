namespace LabBooking.Application.Features.DoorRequests.Dtos;

// 1. Dữ liệu trả về cho Người dùng (Sinh viên/GV) để hiển thị và tạo mã QR
public class DoorRequestQrDto
{
    public Guid RequestId { get; set; }       // ID định danh (quan trọng nhất để mã hóa vào QR)
    public string LabRoomName { get; set; }   // Tên phòng (để hiển thị trên App)
    public string UserFullName { get; set; }  // Tên người sở hữu vé
    public string ValidTimeSlot { get; set; } // Chuỗi hiển thị giờ (VD: "07:00 - 09:00")
    public DateTime StartTime { get; set; }   // Thời gian bắt đầu (để App xử lý logic màu sắc nếu cần)
    public DateTime EndTime { get; set; }     // Thời gian kết thúc
}

// 2. Dữ liệu gửi lên từ máy quét của Bảo vệ
public record VerifyAccessRequest(Guid RequestId);

// 3. Kết quả trả về cho Bảo vệ sau khi quét
public class VerifyAccessResponse
{
    public bool IsValid { get; set; }         // True = Mở cửa / False = Chặn
    public string Message { get; set; } = string.Empty; // Lý do (VD: "Hết giờ", "Chưa duyệt"...)

    // Thông tin bổ sung để bảo vệ đối chiếu (nếu Hợp lệ)
    public string? StudentName { get; set; }
    public string? LabName { get; set; }
    public string? BookingCode { get; set; }
    public string? TimeSlot { get; set; }
}