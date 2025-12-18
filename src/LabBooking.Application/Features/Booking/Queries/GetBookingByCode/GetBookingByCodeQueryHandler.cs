using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Features.Booking.Dtos;

namespace LabBooking.Application.Features.Booking.Queries.GetBookingByCode;

public class GetBookingByCodeQueryHandler(
    IBookingRepository bookingRepository,
    ICurrentUserService currentUserService, // Giả sử service này lấy được Role
    IMapper mapper
    ) : IRequestHandler<GetBookingByCodeQuery, BookingLookupDto>
{
    public async Task<BookingLookupDto> Handle(GetBookingByCodeQuery request, CancellationToken cancellationToken)
    {
        // 1. Validate & Get Data
        if (string.IsNullOrWhiteSpace(request.BookingCode))
            throw new BadRequestException("Vui lòng nhập mã Booking.");

        var booking = await bookingRepository.GetByCodeAsync(request.BookingCode, cancellationToken);

        if (booking == null) throw new NotFoundException(nameof(Booking), request.BookingCode);

        // 2. Security Check cơ bản
        var currentUserId = currentUserService.UserId;
        var currentUserRoles = currentUserService.Roles; // Giả sử bạn có property Role
        // Hoặc nếu dùng Claims: var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

        // Logic cũ: Chỉ cho chủ sở hữu xem
        // Logic mới: Chủ sở hữu HOẶC Manager đều được xem
        bool isManager = currentUserRoles != null &&
                 (currentUserRoles.Contains("Manager") || currentUserRoles.Contains("Admin"));
        bool isOwner = booking.CreatedById == currentUserId;

        if (!isOwner && !isManager)
        {
            // Nếu không phải chủ, cũng không phải sếp -> Chặn
            throw new NotFoundException(nameof(Booking), request.BookingCode);
        }

        // 3. Map dữ liệu cơ bản (Dùng AutoMapper như cũ)
        var dto = mapper.Map<BookingLookupDto>(booking);

        // 4. [MỚI] Logic hiển thị theo Role
        if (isManager)
        {
            // Sử dụng từ khóa 'with' của record để tạo bản copy kèm dữ liệu mới
            dto = dto with
            {
                RequesterFullName = booking.CreatedBy?.FullName,
                RequesterEmail = booking.CreatedBy?.Email,
                RequesterPhoneNumber = booking.CreatedBy?.PhoneNumber
            };
        }

        return dto;
    }
}