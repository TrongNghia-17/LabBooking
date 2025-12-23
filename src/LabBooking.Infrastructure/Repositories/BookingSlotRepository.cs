using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Infrastructure.Repositories
{
    internal class BookingSlotRepository(LabBookingDbContext dbContext) : IBookingSlotRepository
    {
        public async Task<IEnumerable<BookingSlot>> GetBookedSlotsForRoomAsync(
    Guid labRoomId,
    DateOnly startDate, // <-- Đã là DateOnly
    DateOnly endDate,   // <-- Đã là DateOnly
    CancellationToken cancellationToken)
        {
            var busyStatuses = new[] { BookingStatus.Approved };

            // --- SỬA LỖI Ở ĐÂY ---
            //
            // KHÔNG CẦN chuyển đổi hay "đóng dấu" UTC nữa.
            // Chúng ta có thể so sánh DateOnly trực tiếp.
            // Xóa 2 biến startDateTimeUtc và endDateTimeUtc
            //
            // --- KẾT THÚC SỬA ---

            var bookedSlots = await dbContext.BookingSlots
                .Include(bs => bs.Booking)
                .Include(bs => bs.Booking)
            .ThenInclude(b => b.Project)
            .Include(bs => bs.Booking)
            .ThenInclude(b => b.Course)
            .Include(bs => bs.Booking)              // Include Booking cha
            .ThenInclude(b => b.CreatedBy)
                .Include(bs => bs.Slot)
                .Where(bs =>
                    bs.Booking.LabRoomId == labRoomId &&
                    bs.Status == BookingSlotStatus.Active &&
                    // So sánh DateOnly (từ DB) với DateOnly (từ tham số)
                    bs.Date >= startDate &&
                    bs.Date <= endDate &&   // <-- Giờ đã là so sánh ngày, nên dùng <=

                    bs.Booking.Status.HasValue &&
                    
                    busyStatuses.Contains(bs.Booking.Status.Value) 
                )
                .ToListAsync(cancellationToken);

            return bookedSlots;
        }
    }
}
