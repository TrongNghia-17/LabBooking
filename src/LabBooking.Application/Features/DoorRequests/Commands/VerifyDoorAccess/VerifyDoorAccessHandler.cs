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
        {
            return new VerifyAccessResponse { IsValid = false, Message = "Mã QR không tồn tại." };
        }

        // 2. Check trạng thái
        if (doorRequest.Status != DoorRequestStatus.Accepted)
        {
            return new VerifyAccessResponse { IsValid = false, Message = $"Đơn không hợp lệ (Trạng thái: {doorRequest.Status})." };
        }

        // 3. Lấy Booking để check giờ
        var booking = await bookingRepository.GetByCodeAsync(doorRequest.BookingCode, cancellationToken);

        if (booking == null || booking.Slots == null || !booking.Slots.Any())
        {
            return new VerifyAccessResponse { IsValid = false, Message = "Không tìm thấy lịch đặt phòng." };
        }

        // 4. CHECK THỜI GIAN (Time Window Check)
        // Lấy giờ Việt Nam (UTC+7) - BỎ TIMEZONE để so sánh thuần túy
        var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var nowWithTimezone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

        // Chuyển về DateTime không timezone để so sánh
        var now = DateTime.SpecifyKind(nowWithTimezone, DateTimeKind.Unspecified);
        var today = DateOnly.FromDateTime(now);

        bool isTimeValid = false;
        string currentSlotString = "";

        foreach (var slot in booking.Slots)
        {
            // Kiểm tra đúng ngày
            if (slot.Date != today)
                continue;

            // Convert sang DateTime (cũng không có timezone)
            var slotStart = slot.Date.ToDateTime(slot.Slot.StartTime);
            var slotEnd = slot.Date.ToDateTime(slot.Slot.EndTime);

            // Cho phép vào sớm 1 tiếng 30 phút để chuẩn bị và ở lại đến hết giờ
            var allowedStart = slotStart.AddMinutes(-90); // Sớm 90 phút (1h30)
            var allowedEnd = slotEnd;

            // So sánh 2 DateTime đều không có timezone
            if (now >= allowedStart && now <= allowedEnd)
            {
                isTimeValid = true;
                currentSlotString = $"{slot.Date:dd/MM/yyyy} ({slot.Slot.StartTime:HH\\:mm} - {slot.Slot.EndTime:HH\\:mm})";
                break; // Tìm thấy slot hợp lệ thì dừng
            }
        }

        if (!isTimeValid)
        {
            return new VerifyAccessResponse
            {
                IsValid = false,
                Message = "Chưa đến giờ hoặc đã hết giờ vào phòng.",
                LabName = booking.LabRoom?.LabName
            };
        }

        // 5. Success
        return new VerifyAccessResponse
        {
            IsValid = true,
            Message = "Hợp lệ. Mời vào.",
            StudentName = doorRequest.RequestedBy?.FullName,
            LabName = booking.LabRoom?.LabName,
            TimeSlot = currentSlotString
        };
    }
}