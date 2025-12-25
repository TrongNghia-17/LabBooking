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

        // 1. Lấy thông tin Request (đã Include Slot)
        var doorRequest = await doorRequestRepository.GetByIdWithUserAsync(request.Id);

        if (doorRequest == null)
            throw new NotFoundException(nameof(DoorOpeningRequest), request.Id.ToString());

        // 2. CHECK QUYỀN
        if (doorRequest.RequestedById != currentUserId)
        {
            throw new ForbiddenAccessException("Bạn không có quyền lấy mã QR của yêu cầu này.");
        }

        // 3. CHECK TRẠNG THÁI
        if (doorRequest.Status != DoorRequestStatus.Accepted)
        {
            throw new BadRequestException($"Yêu cầu chưa được duyệt (Status: {doorRequest.Status}).");
        }

        // 4. Lấy tên phòng Lab
        var booking = await bookingRepository.GetByCodeAsync(doorRequest.BookingCode, cancellationToken);
        var labName = booking?.LabRoom?.LabName ?? "Phòng Lab";

        // 5. XỬ LÝ THỜI GIAN
        DateTime startDateTime = DateTime.MinValue;
        DateTime endDateTime = DateTime.MinValue;
        string timeDisplay = "Chưa xác định";

        if (doorRequest.Slot != null)
        {
            startDateTime = doorRequest.RequestDate.ToDateTime(doorRequest.Slot.StartTime);
            endDateTime = doorRequest.RequestDate.ToDateTime(doorRequest.Slot.EndTime);

            // Format chuỗi hiển thị
            timeDisplay = $"{doorRequest.RequestDate:dd/MM} ({doorRequest.Slot.StartTime} - {doorRequest.Slot.EndTime})";
        }

        // 6. TRẢ VỀ DTO (Đã sửa tên biến cho khớp file của bạn)
        return new DoorRequestQrDto
        {
            RequestId = doorRequest.Id,
            LabRoomName = labName,
            UserFullName = doorRequest.RequestedBy?.FullName ?? "Unknown User",

            // --- CÁC TRƯỜNG ĐÃ SỬA ---
            StartTime = startDateTime,      // Sửa từ ValidFrom -> StartTime
            EndTime = endDateTime,          // Sửa từ ValidTo -> EndTime
            ValidTimeSlot = timeDisplay     // Sửa từ TimeDisplay -> ValidTimeSlot

            // Bỏ UserStudentCode vì DTO không có
        };
    }
}