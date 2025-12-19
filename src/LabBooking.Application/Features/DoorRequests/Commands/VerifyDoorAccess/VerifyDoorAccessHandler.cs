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
        var now = DateTime.UtcNow;
        bool isTimeValid = false;
        string currentSlotString = "";

        foreach (var slot in booking.Slots)
        {
            // Convert sang DateTime chuẩn để so sánh
            var start = slot.Date.ToDateTime(slot.Slot.StartTime);
            var end = slot.Date.ToDateTime(slot.Slot.EndTime);

            // Cho phép vào sớm 15 phút và trễ tới khi hết giờ
            // (Bạn có thể sửa logic này tùy nghiệp vụ trường)
            if (now >= start.AddMinutes(-15) && now <= end)
            {
                isTimeValid = true;
                currentSlotString = $"{slot.Slot.StartTime} - {slot.Slot.EndTime}";
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