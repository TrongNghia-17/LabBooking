using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Commands.VerifyDoorAccess;

public class VerifyDoorAccessHandler(
    IDoorRequestRepository doorRequestRepository,
    IBookingRepository bookingRepository
    ) : IRequestHandler<VerifyDoorAccessCommand, VerifyAccessResponse>
{
    public async Task<VerifyAccessResponse> Handle(VerifyDoorAccessCommand request, CancellationToken cancellationToken)
    {
        // 1. Tìm Request
        var doorRequest = await doorRequestRepository.GetByIdWithUserAsync(request.RequestId);

        if (doorRequest == null)
            return new VerifyAccessResponse { IsValid = false, Message = "Mã QR không tồn tại." };

        // 2. Check trạng thái
        if (doorRequest.Status != DoorRequestStatus.Accepted)
            return new VerifyAccessResponse { IsValid = false, Message = $"Đơn chưa được duyệt (Status: {doorRequest.Status})." };

        // 3. Lấy thông tin phụ
        var booking = await bookingRepository.GetByCodeAsync(doorRequest.BookingCode, cancellationToken);
        string labName = booking?.LabRoom?.LabName ?? "Phòng Lab";
        string studentName = doorRequest.RequestedBy?.FullName ?? "Sinh viên";

        // 4. KIỂM TRA THỜI GIAN (ĐÃ SỬA LOGIC)
        if (doorRequest.Slot == null)
            return new VerifyAccessResponse { IsValid = false, Message = "Lỗi dữ liệu: Slot null." };

        // --- [FIX 1] CỐ ĐỊNH GIỜ VIỆT NAM (UTC+7) ---
        // Thay vì dùng DateTime.Now (phụ thuộc giờ Server/Docker), ta dùng UTC + 7
        var now = DateTime.UtcNow.AddHours(7);
        // -------------------------------------------

        var requestDate = doorRequest.RequestDate;
        var startDateTime = requestDate.ToDateTime(doorRequest.Slot.StartTime);
        var endDateTime = requestDate.ToDateTime(doorRequest.Slot.EndTime);

        // Cho phép vào sớm 30 phút
        var allowedStart = startDateTime.AddMinutes(-30);
        // Cho phép ra trễ (nếu cần, ví dụ 15p dọn dẹp), ở đây giữ nguyên EndTime
        var allowedEnd = endDateTime;

        // --- [FIX 2] LOGIC SO SÁNH & THÔNG BÁO CHI TIẾT ---

        // Case A: Đến quá sớm
        if (now < allowedStart)
        {
            // Tính thời gian còn lại (phút)
            var minutesToWait = (allowedStart - now).TotalMinutes;

            // Nếu chỉ lệch dưới 1 phút (ví dụ quét lúc 01:29:50), ta có thể châm chước cho qua
            // Bằng cách đổi logic: if (minutesToWait > 1) ...
            // Nhưng để an toàn và chuẩn xác, ta giữ nguyên logic chặn và hiển thị rõ giờ Server.

            return new VerifyAccessResponse
            {
                IsValid = false,
                // Hiển thị giờ hiện tại của Server để user đối chiếu
                Message = $"Chưa đến giờ. Được vào lúc: {allowedStart:HH:mm}. (Hiện tại: {now:HH:mm})",
                LabName = labName
            };
        }

        // Case B: Đến quá muộn (Hết giờ)
        if (now > allowedEnd)
        {
            return new VerifyAccessResponse
            {
                IsValid = false,
                Message = $"Đã hết giờ sử dụng ({endDateTime:HH:mm}).",
                LabName = labName
            };
        }

        // 5. Success
        string timeDisplay = $"{requestDate:dd/MM} ({doorRequest.Slot.StartTime} - {doorRequest.Slot.EndTime})";

        return new VerifyAccessResponse
        {
            IsValid = true,
            Message = "Hợp lệ. Mời vào.",
            StudentName = studentName,
            LabName = labName,
            BookingCode = doorRequest.BookingCode, // Đảm bảo DTO đã có trường này
            TimeSlot = timeDisplay
        };
    }
}