using LabBooking.Application.Features.Booking.Dtos;
using LabBooking.Application.Features.Booking.Queries.GetPendingBooking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Booking.Queries.GetHistoyBooking
{
    internal class GetHistoryBookingQueryHandler(
    IBookingRepository bookingRepository,
    IMapper mapper
    ) : IRequestHandler<GetHistoryBookingQuery, List<BookingResponse>>
    {
        public async Task<List<BookingResponse>> Handle(GetHistoryBookingQuery request, CancellationToken cancellationToken)
        {
            var bookings = await bookingRepository.GetHistoryBookingsAsync(request.UserId);
            return mapper.Map<List<BookingResponse>>(bookings);
        }
    }
}
