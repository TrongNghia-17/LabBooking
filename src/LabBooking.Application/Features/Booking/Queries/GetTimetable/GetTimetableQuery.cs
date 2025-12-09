using LabBooking.Application.Features.Booking.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Booking.Queries.GetTimetable
{
    public record GetTimetableQuery(Guid? UserId) : IRequest<List<BookingHistoryResponse>>;
}
