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
        // 1. Lấy dữ liệu Request (Đã bao gồm Manager nhờ sửa Repository ở trên)
        var doorRequest = await doorRequestRepository.GetByIdWithUserAsync(request.Id);
        if (doorRequest == null) throw new NotFoundException(nameof(DoorOpeningRequest), request.Id.ToString());

        // 2. Lấy dữ liệu Booking (để lấy MainManager phòng Lab nếu chưa có người duyệt)
        var booking = await bookingRepository.GetByCodeAsync(doorRequest.BookingCode, cancellationToken);
        if (booking == null) throw new NotFoundException(nameof(Booking), doorRequest.BookingCode);

        // 3. Xác định quyền người xem
        var currentUserId = currentUserService.UserId;

        // Check A: Người xem có phải là NGƯỜI GỬI (Student) không?
        bool isRequester = doorRequest.RequestedById == currentUserId;

        // Check B: Người xem có phải là QUẢN LÝ PHÒNG (Lab Owner) không?
        bool isLabManager = booking.LabRoom?.MainManagerId == currentUserId;

        // Check C: Người xem có phải là NGƯỜI ĐÃ DUYỆT (Approver) không?
        bool isApprover = doorRequest.ManagerId == currentUserId;

        // Bảo mật: Nếu không dính dáng gì đến đơn này thì chặn
        if (!isRequester && !isLabManager && !isApprover)
        {
            // Tùy chọn: throw new UnauthorizedAccessException("Bạn không có quyền xem.");
        }

        // 4. Map dữ liệu cơ bản
        var dto = new DoorRequestDetailDto
        {
            Id = doorRequest.Id,
            BookingCode = doorRequest.BookingCode,
            RequestDate = doorRequest.RequestDate,
            SlotId = doorRequest.SlotId,
            SlotLabel = doorRequest.Slot?.Label ?? "Không xác định",
            SlotStartTime = doorRequest.Slot?.StartTime ?? default,
            SlotEndTime = doorRequest.Slot?.EndTime ?? default,
            Reason = doorRequest.Reason,
            Status = doorRequest.Status.ToString(),
            RequestTime = doorRequest.RequestTime,
            AcceptedTime = doorRequest.AcceptedTime,
            ManagerNote = doorRequest.ManagerNote,
            LabName = booking.LabRoom?.LabName ?? "Phòng không xác định"
        };

        // 5. XỬ LÝ LOGIC HIỂN THỊ THÔNG TIN LIÊN HỆ (CONTACT INFO)
        User? contactPerson = null;

        if (isRequester)
        {
            // --- TRƯỜNG HỢP: STUDENT ĐANG XEM ---
            // Student cần nhìn thấy thông tin của Manager để liên hệ.

            // Ưu tiên 1: Người đã trực tiếp duyệt đơn này (doorRequest.Manager)
            // Ưu tiên 2: Nếu chưa duyệt, hiển thị chủ phòng Lab (booking.LabRoom.MainManager)
            contactPerson = doorRequest.Manager ?? booking.LabRoom?.MainManager;

            dto.ContactRole = "Quản lý phòng máy";
        }
        else
        {
            // --- TRƯỜNG HỢP: MANAGER ĐANG XEM ---
            // Manager cần nhìn thấy thông tin của Student.
            contactPerson = doorRequest.RequestedBy;

            dto.ContactRole = "Người gửi yêu cầu";
        }

        // 6. Gán dữ liệu User vào DTO an toàn (tránh NullReference)
        if (contactPerson != null)
        {
            dto.ContactName = contactPerson.FullName ?? "Chưa cập nhật tên";
            dto.ContactEmail = contactPerson.Email ?? "N/A";
            dto.ContactPhoneNumber = contactPerson.PhoneNumber ?? "N/A";
        }
        else
        {
            dto.ContactName = "Chưa có thông tin";
            dto.ContactEmail = "N/A";
            dto.ContactPhoneNumber = "N/A";
        }

        return dto;
    }
}