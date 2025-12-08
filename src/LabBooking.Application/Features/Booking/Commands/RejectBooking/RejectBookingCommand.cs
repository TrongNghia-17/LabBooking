using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Booking.Commands.RejectBooking
{
    public record RejectBookingCommand(
    Guid BookingId,
    Guid ManagerId,
    string? reason
) : IRequest<bool>;
}
