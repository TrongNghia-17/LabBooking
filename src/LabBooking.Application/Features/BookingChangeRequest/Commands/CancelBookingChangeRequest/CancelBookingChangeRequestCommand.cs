using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Commands.CancelBookingChangeRequest
{
    public record CancelBookingChangeRequestCommand(Guid Id, Guid UserId) : IRequest<bool>;
}
