using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Commands.VerifyDoorAccess;

public class VerifyDoorAccessHandler(
    IDoorRequestRepository doorRequestRepository,
    IBookingRepository bookingRepository
    ) : IRequestHandler<VerifyDoorAccessCommand, VerifyAccessResponse>
{
    public async Task<VerifyAccessResponse> Handle(VerifyDoorAccessCommand request, CancellationToken cancellationToken)
    {
        // 1. Tìm Request (Repository đã Include Slot, User, Manager rồi)
        var doorRequest = await doorRequestRepository.GetByIdWithUserAsync(request.RequestId);

        if (doorRequest == null)
        {
            return new VerifyAccessResponse { IsValid = false, Message = "Mã QR không tồn tại." };
        }

        // 2. Check trạng thái đơn
        if (doorRequest.Status != DoorRequestStatus.Accepted)
        {
            return new VerifyAccessResponse { IsValid = false, Message = $"Đơn không hợp lệ (Trạng thái: {doorRequest.Status})." };
        }

        // 3. Lấy tên phòng Lab (để hiển thị cho bảo vệ)
        var booking = await bookingRepository.GetByCodeAsync(doorRequest.BookingCode, cancellationToken);
        string labName = booking?.LabRoom?.LabName ?? "Phòng Lab";
        string studentName = doorRequest.RequestedBy?.FullName ?? "Sinh viên";

        // 4. KIỂM TRA THỜI GIAN (LOGIC MỚI - QUAN TRỌNG)
        // Không lặp qua booking.Slots nữa, mà check trực tiếp vào doorRequest.Slot

        if (doorRequest.Slot == null)
        {
            return new VerifyAccessResponse { IsValid = false, Message = "Lỗi dữ liệu: Không tìm thấy thông tin Slot trong yêu cầu." };
        }

        // Thời điểm hiện tại (Server time)
        var now = DateTime.Now;

        // Tính toán thời gian cho phép vào
        // Combine DateOnly + TimeOnly -> DateTime
        var requestDate = doorRequest.RequestDate;
        var startDateTime = requestDate.ToDateTime(doorRequest.Slot.StartTime);
        var endDateTime = requestDate.ToDateTime(doorRequest.Slot.EndTime);

        // Quy định: Được vào sớm 30 phút (hoặc 90 phút tùy bạn config) để chuẩn bị
        // Ví dụ: Ca bắt đầu 07:00 -> 06:30 được vào.
        var allowedStart = startDateTime.AddMinutes(-30);
        var allowedEnd = endDateTime; // Hết giờ là phải ra, hoặc cho trễ thêm 15p dọn dẹp tùy bạn: .AddMinutes(15)

        // LOGIC SO SÁNH
        if (now < allowedStart)
        {
            return new VerifyAccessResponse
            {
                IsValid = false,
                Message = $"Chưa đến giờ vào. Vui lòng quay lại lúc {allowedStart:HH:mm}.",
                LabName = labName
            };
        }

        if (now > allowedEnd)
        {
            return new VerifyAccessResponse
            {
                IsValid = false,
                Message = "Đã hết giờ sử dụng phòng.",
                LabName = labName
            };
        }

        // 5. Success - Hợp lệ
        // Format chuỗi hiển thị: "25/12 (07:00 - 09:00)"
        string timeDisplay = $"{requestDate:dd/MM} ({doorRequest.Slot.StartTime} - {doorRequest.Slot.EndTime})";

        return new VerifyAccessResponse
        {
            IsValid = true,
            Message = "Hợp lệ. Mời vào.",
            StudentName = studentName,
            LabName = labName,
            BookingCode = doorRequest.BookingCode,
            TimeSlot = timeDisplay
        };
    }
}