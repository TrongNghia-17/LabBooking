using LabBooking.Application.Features.Booking.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.HistoryBooking.Queries.GetMyBookingHistory
{
    public record GetMyBookingHistoryQuery(Guid UserId) : IRequest<List<BookingResponse>>;
}
