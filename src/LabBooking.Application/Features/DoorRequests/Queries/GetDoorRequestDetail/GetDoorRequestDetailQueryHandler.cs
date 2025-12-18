using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetDoorRequestDetail;

public class GetDoorRequestDetailQueryHandler(
    IDoorRequestRepository doorRequestRepository,
    IBookingRepository bookingRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<GetDoorRequestDetailQuery, DoorRequestDetailDto>
{
    public async Task<DoorRequestDetailDto> Handle(GetDoorRequestDetailQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy dữ liệu
        var doorRequest = await doorRequestRepository.GetByIdWithUserAsync(request.Id);
        if (doorRequest == null) throw new NotFoundException(nameof(DoorOpeningRequest), request.Id.ToString());

        var booking = await bookingRepository.GetByCodeAsync(doorRequest.BookingCode, cancellationToken);
        if (booking == null) throw new NotFoundException(nameof(Booking), doorRequest.BookingCode);

        // 2. Xác định vai trò của người đang xem
        var currentUserId = currentUserService.UserId;

        // Check A: Có phải là Manager của phòng Lab này không?
        var labManagerId = booking.LabRoom?.MainManagerId;
        bool isManager = labManagerId != null && labManagerId == currentUserId;

        // Check B: Có phải là người tạo yêu cầu này không (Student/Lecturer)?
        bool isRequester = doorRequest.RequestedById == currentUserId;

        // 3. SECURITY CHECK: Nếu không phải Manager, cũng không phải chủ đơn -> CÚT
        if (!isManager && !isRequester)
        {
            throw new ForbiddenAccessException("Bạn không có quyền xem yêu cầu này.");
        }

        // 4. Chuẩn bị DTO cơ bản
        var dto = new DoorRequestDetailDto
        {
            Id = doorRequest.Id,
            BookingCode = doorRequest.BookingCode,
            Reason = doorRequest.Reason,
            Status = doorRequest.Status.ToString(),
            RequestTime = doorRequest.RequestTime,
            AcceptedTime = doorRequest.AcceptedTime,
            ManagerNote = doorRequest.ManagerNote,
            LabName = booking.LabRoom?.LabName ?? "Phòng không xác định"
        };

        // 5. XỬ LÝ HIỂN THỊ THÔNG TIN LIÊN HỆ (Logic bạn yêu cầu)
        if (isManager)
        {
            // CASE 1: Manager đang xem -> Hiển thị thông tin người gửi (Student/Lecturer)
            dto.RequestedByName = doorRequest.RequestedBy?.FullName ?? "N/A";
            dto.RequestedByEmail = doorRequest.RequestedBy?.Email ?? "N/A";
            dto.RequestedByPhoneNumber = doorRequest.RequestedBy?.PhoneNumber ?? "N/A";
        }
        else
        {
            // CASE 2: Student/Lecturer đang xem -> Hiển thị thông tin Manager (để họ biết đường liên hệ)
            // Lấy thông tin từ booking.LabRoom.MainManager (đã Include ở Repo)
            var managerInfo = booking.LabRoom?.MainManager;

            dto.RequestedByName = managerInfo?.FullName ?? "Unknown Manager";
            dto.RequestedByEmail = managerInfo?.Email ?? "N/A";
            dto.RequestedByPhoneNumber = managerInfo?.PhoneNumber ?? "N/A";
        }

        return dto;
    }
}