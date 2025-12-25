using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Features.Booking.Dtos;

namespace LabBooking.Application.Features.Booking.Queries.GetBookingByCode;

public class GetBookingByCodeQueryHandler(
    IBookingRepository bookingRepository,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IRequestHandler<GetBookingByCodeQuery, BookingLookupDto>
{
    public async Task<BookingLookupDto> Handle(GetBookingByCodeQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy booking với đầy đủ slots (Repository đã Include Slots và Slot)
        var booking = await bookingRepository.GetByCodeAsync(request.BookingCode, cancellationToken)
            ?? throw new NotFoundException($"Không tìm thấy đơn đặt phòng với mã: {request.BookingCode}");

        // 2. Security Check
        var currentUserId = currentUserService.UserId;
        var currentUserRoles = currentUserService.Roles;

        bool isManager = currentUserRoles != null &&
                 (currentUserRoles.Contains("Manager") || currentUserRoles.Contains("Admin"));
        bool isOwner = booking.CreatedById == currentUserId;

        if (!isOwner && !isManager)
        {
            throw new NotFoundException(nameof(Booking), request.BookingCode);
        }

        // 3. Lọc và nhóm các slots Active theo ngày
        var activeSlots = booking.Slots?
            .Where(s => s.Status == BookingSlotStatus.Active)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.Slot?.StartTime)
            .ToList() ?? new List<BookingSlot>();

        // 4. Nhóm theo ngày và tạo DTO
        var dateSlots = activeSlots
            .GroupBy(s => s.Date)
            .Select(g => new BookingDateSlotDto
            {
                Date = g.Key,
                Slots = g.Select(s => new SlotInfoDto
                {
                    SlotId = s.SlotId,
                    SlotLabel = s.Slot?.Label ?? "N/A",
                    StartTime = s.Slot?.StartTime ?? TimeOnly.MinValue,
                    EndTime = s.Slot?.EndTime ?? TimeOnly.MinValue
                }).ToList()
            })
            .ToList();

        // 5. Tạo DTO cơ bản
        var dto = new BookingLookupDto
        {
            Id = booking.Id,
            BookingCode = booking.QrCodeString ?? string.Empty,
            LabName = booking.LabRoom?.LabName ?? string.Empty,
            DateSlots = dateSlots,

            // Nếu là Manager -> Hiển thị thông tin người đặt
            RequesterFullName = isManager ? booking.CreatedBy?.FullName : null,
            RequesterEmail = isManager ? booking.CreatedBy?.Email : null,
            RequesterPhoneNumber = isManager ? booking.CreatedBy?.PhoneNumber : null
        };

        return dto;
    }
}