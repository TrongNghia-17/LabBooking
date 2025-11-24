using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Repositories
{
    public interface IBookingChangeRequestRepository
    {
        Task<BookingChangeRequest> CreateAsync(BookingChangeRequest request);
        Task<bool> IsBookingOwnerAndApprovedAsync(Guid bookingId, Guid userId);
        Task<List<BookingChangeRequest>> GetPendingRequestsAsync(Guid? labId);
    }
}
