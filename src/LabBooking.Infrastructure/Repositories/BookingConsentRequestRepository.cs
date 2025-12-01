using LabBooking.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Infrastructure.Repositories
{
    internal class BookingConsentRequestRepository(LabBookingDbContext dbContext) : IBookingConsentRequestRepository
    {
        public async Task<BookingConsentRequest?> GetByIdWithBookingAndSlotsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await dbContext.BookingConsentRequests
                .Include(c => c.Booking)
                    .ThenInclude(b => b.Slots)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        // --- LOGIC MỚI ĐƯỢC CHUYỂN XUỐNG ĐÂY ---
        public async Task ConfirmConsentCancelAsync(Guid consentId, CancellationToken cancellationToken)
        {
            // 1. Tìm kiếm dữ liệu (Re-use logic query nếu cần hoặc viết trực tiếp)
            var consentRequest = await dbContext.BookingConsentRequests
                .Include(c => c.Booking)
                    .ThenInclude(b => b.Slots)
                .FirstOrDefaultAsync(c => c.Id == consentId, cancellationToken);

            // 2. Validate
            if (consentRequest == null)
                throw new BadRequestException("Không tìm thấy yêu cầu xác nhận.");

            if (consentRequest.Status != ConsentStatus.Pending)
                throw new BadRequestException("Yêu cầu này đã được xử lý trước đó.");

            // 3. Xử lý logic: Cập nhật Consent
            consentRequest.Status = ConsentStatus.AcceptedCancel;
            consentRequest.ResolvedAt = DateTime.UtcNow;

            // 4. Xử lý logic: Kiểm tra Booking gốc (Booking cha)
            // Kiểm tra xem Booking cha còn slot nào Active không?
            var hasActiveSlots = consentRequest.Booking.Slots
                .Any(s => s.Status == BookingSlotStatus.Active);

            // Nếu không còn slot nào Active -> Hủy luôn Booking cha
            if (!hasActiveSlots)
            {
                consentRequest.Booking.Status = BookingStatus.Cancelled;
                // consentRequest.Booking.CancelReason = "Người dùng đồng ý hủy do bị chiếm lịch.";
            }

            // 5. Save Changes
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
