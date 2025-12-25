namespace LabBooking.Application.Features.Booking.Queries.GetBookingByCode;

public class GetBookingByCodeQueryValidator : AbstractValidator<GetBookingByCodeQuery>
{
    public GetBookingByCodeQueryValidator()
    {
        RuleFor(x => x.BookingCode)
            .NotEmpty().WithMessage("Vui lòng nhập mã Booking.") // Message lỗi khi rỗng
            .MinimumLength(3).WithMessage("Mã Booking không hợp lệ (quá ngắn)."); // Validate thêm nếu cần
    }
}
