using LabBooking.Application.Features.Slots.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Slots.Queries.GetAllSlots
{
    /// <remarks>
    /// This query returns a <see cref="IEnumerable{SlotResponse}"/>.
    /// </remarks>
    public record GetAllSlotsQuery() : IRequest<IEnumerable<SlotResponse>>;
}
