using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Commands.RejectBookingChangeRequest
{
    public record RejectBookingChangeRequestCommand(
        Guid BookingId,
        Guid ManagerId,
        string? Reason
    ) : IRequest<bool>;
}
