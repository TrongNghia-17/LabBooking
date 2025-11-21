using LabBooking.Application.Features.Booking.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Booking.Queries.GetBookingById
{
    public class GetBookingByIdQueryHandler(
    IBookingRepository bookingRepository,
    IMapper mapper
    ) : IRequestHandler<GetBookingByIdQuery, BookingResponse>
    {
        public async Task<BookingResponse> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            var booking = await bookingRepository.GetBookingDetailsAsync(request.Id);

            if (booking == null)
                throw new NotFoundException(nameof(Booking), request.Id.ToString());

            return mapper.Map<BookingResponse>(booking);
        }
    }
}
