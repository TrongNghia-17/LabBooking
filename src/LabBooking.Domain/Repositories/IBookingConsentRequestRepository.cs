using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Repositories
{
    public interface IBookingConsentRequestRepository
    {
        public Task<BookingConsentRequest?> GetByIdWithBookingAndSlotsAsync(Guid id, CancellationToken cancellationToken);
        Task ConfirmConsentCancelAsync(Guid consentId, CancellationToken cancellationToken);
    }
}
