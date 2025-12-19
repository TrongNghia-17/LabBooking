using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetDoorRequestQr;

public class GetDoorRequestQrQueryHandler(
    IDoorRequestRepository doorRequestRepository,
    IBookingRepository bookingRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<GetDoorRequestQrQuery, DoorRequestQrDto>
{
    public async Task<DoorRequestQrDto> Handle(GetDoorRequestQrQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;

        // 1. Lấy thông tin Request
        var doorRequest = await doorRequestRepository.GetByIdWithUserAsync(request.Id);

        if (doorRequest == null)
            throw new NotFoundException(nameof(DoorOpeningRequest), request.Id.ToString());

        // 2. CHECK QUYỀN: Chỉ người tạo đơn mới được lấy
        if (doorRequest.RequestedById != currentUserId)
        {
            throw new ForbiddenAccessException("Bạn không có quyền lấy mã QR của yêu cầu này.");
        }

        // 3. CHECK TRẠNG THÁI
        if (doorRequest.Status != DoorRequestStatus.Accepted)
        {
            throw new BadRequestException($"Yêu cầu chưa được duyệt (Status: {doorRequest.Status}).");
        }

        // 4. Lấy thông tin Booking
        var booking = await bookingRepository.GetByCodeAsync(doorRequest.BookingCode, cancellationToken);

        // 5. Xử lý hiển thị thời gian
        string timeDisplay = "Chưa xác định";
        DateTime start = DateTime.MinValue;
        DateTime end = DateTime.MinValue;

        if (booking?.Slots != null && booking.Slots.Any())
        {
            // Lấy slot đầu tiên làm chuẩn hiển thị
            var firstSlot = booking.Slots.OrderBy(s => s.Date).ThenBy(s => s.Slot.StartTime).First();

            // Logic convert ngày giờ
            start = firstSlot.Date.ToDateTime(firstSlot.Slot.StartTime);
            end = firstSlot.Date.ToDateTime(firstSlot.Slot.EndTime);

            timeDisplay = $"{firstSlot.Date:dd/MM} ({firstSlot.Slot.StartTime} - {firstSlot.Slot.EndTime})";
        }

        return new DoorRequestQrDto
        {
            RequestId = doorRequest.Id,
            LabRoomName = booking?.LabRoom?.LabName ?? "Phòng Lab",
            UserFullName = doorRequest.RequestedBy?.FullName ?? "Người dùng",
            ValidTimeSlot = timeDisplay,
            StartTime = start,
            EndTime = end
        };
    }
}