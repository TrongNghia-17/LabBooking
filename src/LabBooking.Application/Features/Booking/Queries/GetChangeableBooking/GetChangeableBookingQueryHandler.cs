using LabBooking.Application.Features.Booking.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Booking.Queries.GetChangeableBooking
{
    public class GetChangeableBookingQueryHandler(
    IBookingRepository bookingRepository,
    IMapper mapper
    ) : IRequestHandler<GetChangeableBookingsQuery, List<BookingResponse>>
    {
        public async Task<List<BookingResponse>> Handle(GetChangeableBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await bookingRepository.GetChangeableBookingsAsync(request.UserId);
            var bookingDtos = mapper.Map<List<BookingResponse>>(bookings);
            return bookingDtos;
        }
    }
}
