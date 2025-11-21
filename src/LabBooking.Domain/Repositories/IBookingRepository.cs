using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking> CreateBookingAsync(Booking newBooking);
        Task<IEnumerable<Booking>> GetChangeableBookingsAsync(Guid userId);
        Task<Booking?> GetBookingDetailsAsync(Guid id);
    }
}
