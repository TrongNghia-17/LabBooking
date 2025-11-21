using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Infrastructure.Repositories
{
    internal class BookingChangeRequestRepository(LabBookingDbContext dbContext) : IBookingChangeRequestRepository
    {
        public async Task<BookingChangeRequest> CreateAsync(BookingChangeRequest entity)
        {
            dbContext.BookingChangeRequests.Add(entity);
            //await dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> IsBookingOwnerAndApprovedAsync(Guid bookingId, Guid userId)
        {
            // Logic:
            // 1. Phải đúng BookingId
            // 2. Phải đúng người tạo (CreatedById == userId)
            // 3. (Tùy chọn) Booking phải đang Approved mới được làm đơn xin đổi

            return await dbContext.Bookings.AnyAsync(b =>
                b.Id == bookingId &&
                b.CreatedById == userId &&
                b.Status == BookingStatus.Approved
            );
        }
    }
}
