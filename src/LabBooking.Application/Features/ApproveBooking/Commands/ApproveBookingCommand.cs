using LabBooking.Application.Features.ApproveBooking.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.ApproveBooking.Commands
{
    public record ApproveBookingCommand(
    Guid BookingId,
    Guid ManagerId
) : IRequest<bool>;
}
