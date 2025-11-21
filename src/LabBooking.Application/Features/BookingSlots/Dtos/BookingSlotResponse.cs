using LabBooking.Domain.Entities;
using Medo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingSlots.Dtos
{
    public record BookingSlotResponse
    (
        Guid Id,
        Guid BookingId,
        DateOnly Date,
        Guid SlotId,
        UnavailableReason Reason,
        int Priority
    );
}
