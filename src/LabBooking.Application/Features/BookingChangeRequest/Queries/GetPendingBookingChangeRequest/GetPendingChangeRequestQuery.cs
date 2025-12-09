using LabBooking.Application.Features.BookingChangeRequest.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Queries.GetPendingBookingChangeRequest
{
    public record GetPendingChangeRequestQuery(Guid? UserId) : IRequest<List<BookingChangeRequestResponse>>;

}
