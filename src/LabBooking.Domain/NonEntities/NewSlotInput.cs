using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.NonEntities
{
    public record NewSlotInput(DateOnly Date, Guid SlotId);
}
