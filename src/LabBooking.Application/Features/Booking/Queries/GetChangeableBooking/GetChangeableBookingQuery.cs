using LabBooking.Application.Features.Booking.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Booking.Queries.GetChangeableBooking
{
    public record GetChangeableBookingsQuery(Guid UserId) : IRequest<List<BookingResponse>>;
}
