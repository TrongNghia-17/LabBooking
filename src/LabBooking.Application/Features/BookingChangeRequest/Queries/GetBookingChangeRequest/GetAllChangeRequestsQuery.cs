using LabBooking.Application.Features.BookingChangeRequest.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Queries.GetBookingChangeRequest
{
    public record GetAllChangeRequestsQuery(Guid? UserId) : IRequest<List<BookingChangeRequestResponse>>;
}
