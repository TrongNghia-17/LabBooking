using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Infrastructure.Repositories
{
    internal class BookingRepository(LabBookingDbContext dbContext) : IBookingRepository
    {
        public async Task<Booking> CreateBookingAsync(Booking newBooking)
        {
            // 1. Bắt đầu Transaction (An toàn dữ liệu tuyệt đối)
            //using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // --- GIAI ĐOẠN 1: LỌC SƠ BỘ (BROAD FILTER) TẠI DATABASE ---

                // Lấy danh sách các ID và Date cần check
                var reqSlotIds = newBooking.Slots!.Select(s => s.SlotId).Distinct().ToList();
                var reqDates = newBooking.Slots!.Select(s => s.Date).Distinct().ToList();

                // Query này SQL hiểu được và chạy rất nhanh (dùng Index)
                // Nó sẽ lấy "dư" một chút (những slot trùng ngày nhưng khác giờ, hoặc trùng giờ nhưng khác ngày)
                var candidateConflicts = await dbContext.BookingSlots
                    .Include(bs => bs.Booking) // Include để lấy Priority của booking cũ
                    .Where(bs =>
                        bs.Booking.LabRoomId == newBooking.LabRoomId && // Cùng phòng
                        reqSlotIds.Contains(bs.SlotId) &&               // SQL: WHERE SlotId IN (...)
                        reqDates.Contains(bs.Date) &&                   // SQL: AND Date IN (...)

                        // Chỉ quan tâm các slot đang chiếm chỗ (Active)
                        (bs.Booking.Status == BookingStatus.Approved)
                    )
                    .ToListAsync(); // <--- Kéo dữ liệu về RAM tại đây

                // --- GIAI ĐOẠN 2: LỌC TINH (EXACT MATCH) TẠI RAM ---

                // Dùng C# để lọc ra những slot trùng KHÍT cả (SlotId + Date)
                // Logic này EF Core không dịch được, nhưng chạy trên RAM với list nhỏ thì cực nhanh (<1ms)
                var realConflicts = candidateConflicts
                    .Where(dbSlot => newBooking.Slots.Any(newSlot =>
                        newSlot.SlotId == dbSlot.SlotId &&
                        newSlot.Date == dbSlot.Date
                    ))
                    .ToList();

                // --- GIAI ĐOẠN 3: XỬ LÝ XUNG ĐỘT (OVERRIDE LOGIC) ---

                var hasConflict = realConflicts.Any();

                if (hasConflict)
                {
                    // Check quyền
                    foreach (var conflict in realConflicts)
                    {
                        var oldPriority = conflict.Booking?.Priority ?? BookingPriority.Standard;
                        if (newBooking.Priority >= oldPriority) // Số càng nhỏ càng to
                        {
                            throw new InvalidOperationException($"Xung đột ngày {conflict.Date}...");
                        }
                    }

                    // === QUYẾT ĐỊNH: KHÔNG XÓA CŨ - KHÔNG THÊM MỚI ===

                    // 1. Serialize danh sách slot mong muốn lại
                    newBooking.PendingSlotsJson = JsonSerializer.Serialize(newBooking.Slots);

                    // 2. Xóa sạch list Slots để EF Core KHÔNG insert vào bảng BookingSlot
                    newBooking.Slots = null; // <--- KEY POINT

                    // 3. Set trạng thái Pending
                    newBooking.Status = BookingStatus.Pending;
                }
                else
                {
                    // Nếu KHÔNG có xung đột -> Insert thẳng vào BookingSlot luôn (nếu logic cho phép tự duyệt)
                    // Hoặc nếu quy trình bắt buộc duyệt -> Cũng làm y chang như trên (lưu JSON, Slots=null).

                    // Giả sử Priority Booking luôn cần duyệt:
                    newBooking.PendingSlotsJson = JsonSerializer.Serialize(newBooking.Slots);
                    newBooking.Slots = null;
                    newBooking.Status = BookingStatus.Pending;
                }

                // --- GIAI ĐOẠN 4: LƯU MỚI ---

                // EF Core thông minh sẽ tự lưu Booking -> tự lưu BookingSlots -> tự lưu ExternalEquipments
                dbContext.Bookings.Add(newBooking);

                //await dbContext.SaveChangesAsync();

                // Commit Transaction: Chốt đơn!
                //await transaction.CommitAsync();
                if (newBooking.Type == BookingType.Teaching && newBooking.CourseId.HasValue)
                {
                    await dbContext.Entry(newBooking).Reference(b => b.Course).LoadAsync();
                }

                // Nếu là Project -> Load Project (Nếu chưa có)
                if (newBooking.Type == BookingType.Project && newBooking.ProjectId.HasValue)
                {
                    await dbContext.Entry(newBooking).Reference(b => b.Project).LoadAsync();
                }

                // Nếu là Priority -> Load PriorityDetail
                if (newBooking.Type == BookingType.UniversityEvent && newBooking.BookingPriorityDetailId.HasValue)
                {
                    await dbContext.Entry(newBooking).Reference(b => b.BookingPriorityDetail).LoadAsync();
                }

                return newBooking;
            }
            catch
            {
                // Có bất kỳ lỗi gì -> Hoàn tác mọi thứ (kể cả việc xóa slot cũ)
                //await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Booking>> GetChangeableBookingsAsync(Guid userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            return await dbContext.Bookings
                .Include(b => b.LabRoom) // Include để lấy tên phòng
                .Include(b => b.Slots)   // Include để check ngày
                .Where(b =>
                    b.CreatedById == userId &&               // 1. Của mình
                    b.Status == BookingStatus.Approved &&    // 2. Đã duyệt
                    b.Slots.Any(s => s.Date >= today) &&     // 3. Còn slot tương lai

                    // 4. (Optional) Không có yêu cầu đổi nào đang chờ duyệt
                    !dbContext.BookingChangeRequests.Any(cr =>
                        cr.BookingId == b.Id &&
                        cr.Status == BookingChangeRequestStatus.Pending
                    )
                )
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingDetailsAsync(Guid id)
        {
            return await dbContext.Bookings
                .Include(b => b.LabRoom)             // Lấy tên phòng
                .Include(b => b.Slots)               // Lấy các slot đã đặt
                .Include(b => b.Course)              // Lấy tên môn (nếu Teaching)
                .Include(b => b.Project)             // Lấy dự án (nếu Project)
                .Include(b => b.BookingPriorityDetail) // Lấy lý do ưu tiên
                .Include(b => b.ExternalEquipments)  // Lấy thiết bị
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
