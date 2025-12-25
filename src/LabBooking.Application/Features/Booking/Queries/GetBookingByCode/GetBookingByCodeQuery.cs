using LabBooking.Application.Features.Booking.Dtos;

namespace LabBooking.Application.Features.Booking.Queries.GetBookingByCode;

public record GetBookingByCodeQuery(string BookingCode) : IRequest<BookingLookupDto>;
