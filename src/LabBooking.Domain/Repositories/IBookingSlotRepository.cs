using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Repositories
{
    public interface IBookingSlotRepository
    {
        Task<IEnumerable<BookingSlot>> GetBookedSlotsForRoomAsync(
                    Guid labRoomId,
                    DateOnly startDate,
                    DateOnly endDate,
                    CancellationToken cancellationToken);
    }
}
