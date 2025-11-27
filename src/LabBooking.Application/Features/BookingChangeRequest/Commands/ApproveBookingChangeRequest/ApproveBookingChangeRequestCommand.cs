using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingChangeRequest.Commands.ApproveBookingChangeRequest
{
    public record ApproveBookingChangeRequestCommand(
        Guid BookingChangeRequestId,
        Guid ManagerId
    ) : IRequest<bool>;
}
