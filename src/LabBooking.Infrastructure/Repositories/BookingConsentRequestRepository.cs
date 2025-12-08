using LabBooking.Domain.Exceptions;
using LabBooking.Domain.NonEntities;
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
                .Include(c => c.Booking)
                    .ThenInclude(b => b.LabRoom)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        // --- LOGIC MỚI ĐƯỢC CHUYỂN XUỐNG ĐÂY ---
        public async Task ConfirmConsentCancelAsync(Guid consentId, CancellationToken cancellationToken)
        {
            // 1. Tìm kiếm dữ liệu (Re-use logic query nếu cần hoặc viết trực tiếp)
            var consentRequest = await GetByIdWithBookingAndSlotsAsync(consentId, cancellationToken);

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

        public async Task CreateRescheduleRequestAsync(Guid consentId, List<NewSlotInput> newSlots, CancellationToken cancellationToken)
        {
            // 1. Lấy thông tin Consent hiện tại
            var consentRequest = await GetByIdWithBookingAndSlotsAsync(consentId, cancellationToken);

            if (consentRequest == null)
                throw new KeyNotFoundException("Không tìm thấy yêu cầu xác nhận.");

            // 2. Tạo BookingChangeRequest (Yêu cầu thay đổi - Chờ Manager duyệt)
            var changeRequest = new BookingChangeRequest
            {
                Id = Guid.NewGuid(),
                BookingId = consentRequest.BookingId,
                RequestedById = consentRequest.CreatedById, // Người tạo Booking là người yêu cầu
                Status = BookingChangeRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                // Các trường khác (Title, Description...) để null nghĩa là không đổi
            };

            // 3. Map danh sách slot từ NewSlotInput vào BookingChangeRequestSlot
            if (newSlots != null && newSlots.Any())
            {
                changeRequest.NewSlots = newSlots.Select(s => new BookingChangeRequestSlot
                {
                    Id = Guid.NewGuid(),
                    // BookingChangeRequestId tự động được gán
                    SlotId = s.SlotId,

                    // [QUAN TRỌNG] Convert DateOnly -> DateTime để lưu vào DB
                    Date = s.Date
                }).ToList();
            }

            // 4. Cập nhật trạng thái Consent -> Rescheduled (User đã phản hồi xong)
            consentRequest.Status = ConsentStatus.Rescheduled;
            consentRequest.ResolvedAt = DateTime.UtcNow;

            // 5. Lưu tất cả vào DB (ChangeRequest + NewSlots + Update Consent)
            dbContext.BookingChangeRequests.Add(changeRequest);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Dictionary<Guid, string>> GetStatusesAsync(List<Guid> consentIds, CancellationToken cancellationToken)
        {
            if (consentIds == null || consentIds.Count == 0)
                return new Dictionary<Guid, string>();

            // Query một lần duy nhất cho tất cả ID
            // Lưu ý: .ToString() trên server-side có thể không được EF Core hỗ trợ tùy DB Provider.
            // Cách an toàn nhất là lấy Enum về rồi .ToString() ở client-side (như dưới đây)
            var list = await dbContext.BookingConsentRequests
                .Where(c => consentIds.Contains(c.Id))
                .Select(c => new { c.Id, c.Status })
                .ToListAsync(cancellationToken);

            return list.ToDictionary(
                x => x.Id,
                x => x.Status.ToString() // Convert Enum sang String tại đây (Client-side evaluation)
            );
        }
    }
}
