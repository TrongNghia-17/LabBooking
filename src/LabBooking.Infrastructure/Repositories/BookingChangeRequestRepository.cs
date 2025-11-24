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
            await dbContext.SaveChangesAsync();
            return await dbContext.BookingChangeRequests
                .Include(r => r.NewSlots) // Load slot mới
                .Include(r => r.Booking)  // Load booking gốc
                .ThenInclude(b => b.LabRoom) // Load phòng từ booking gốc
                .FirstOrDefaultAsync(r => r.Id == entity.Id);
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

        public async Task<List<BookingChangeRequest>> GetPendingRequestsAsync(Guid? labId)
        {
            var query = dbContext.BookingChangeRequests
                .Include(r => r.NewSlots)        // Lấy danh sách slot mới
                .Include(r => r.Booking)         // [QUAN TRỌNG] Join sang Booking gốc
                    .ThenInclude(b => b.LabRoom) // Lấy thông tin phòng để hiển thị/lọc
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Slots)  // Lấy Course cũ (nếu cần hiển thị)
                .Where(r => r.Status == BookingChangeRequestStatus.Pending);

            // Lọc theo LabId (dựa vào Booking gốc)
            if (labId.HasValue)
            {
                query = query.Where(r => r.Booking.LabRoomId == labId);
            }

            // Sắp xếp: Đơn cũ nhất lên đầu (FIFO) để duyệt trước
            return await query
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
