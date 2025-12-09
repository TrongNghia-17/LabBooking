using LabBooking.Application.Features.Booking.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Booking.Queries.GetPendingBooking
{
    public class GetPendingBookingsQueryHandler(
    IBookingRepository bookingRepository,
    IMapper mapper
    ) : IRequestHandler<GetPendingBookingsQuery, List<BookingResponse>>
    {
        public async Task<List<BookingResponse>> Handle(GetPendingBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await bookingRepository.GetPendingBookingsAsync(request.UserId);
            return mapper.Map<List<BookingResponse>>(bookings);
        }
    }
}
