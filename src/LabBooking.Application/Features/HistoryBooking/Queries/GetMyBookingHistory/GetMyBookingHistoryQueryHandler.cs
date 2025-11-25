using LabBooking.Application.Features.Booking.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.HistoryBooking.Queries.GetMyBookingHistory
{
    public class GetMyBookingHistoryQueryHandler(
    IBookingRepository bookingRepository,
    IMapper mapper
    ) : IRequestHandler<GetMyBookingHistoryQuery, List<BookingResponse>>
    {
        public async Task<List<BookingResponse>> Handle(GetMyBookingHistoryQuery request, CancellationToken token)
        {
            var bookings = await bookingRepository.GetHistoryByUserIdAsync(request.UserId);
            var bookingResponses = mapper.Map<List<BookingResponse>>(bookings);
            return bookingResponses;
        }
    }
}
