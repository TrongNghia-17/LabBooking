using LabBooking.Application.Interfaces.Notifications;
using LabBooking.Domain.Exceptions;

namespace LabBooking.Infrastructure.Repositories
{
    internal class BookingRepository(LabBookingDbContext dbContext, IUserDeviceRepository userDeviceRepository,
    INotificationService notificationService, INotificationRepository notificationRepo) : IBookingRepository
    {
        public async Task<Booking> CreateBookingAsync(Booking newBooking)
        {
            var pushQueue = new List<PushNotificationData>();
            if (newBooking.CreatedAt.HasValue)
            {
                // Nếu có: Lấy giá trị ra (.Value), ép kiểu UTC, rồi gán lại
                newBooking.CreatedAt = DateTime.SpecifyKind(newBooking.CreatedAt.Value, DateTimeKind.Utc);
            }
            else
            {
                // Nếu null: Gán luôn giờ hiện tại chuẩn UTC (cho chắc ăn)
                newBooking.CreatedAt = DateTime.UtcNow;
            }
            // 1. Bắt đầu Transaction (An toàn dữ liệu tuyệt đối)
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var labInfo = await dbContext.LabRooms
                    .Where(l => l.Id == newBooking.LabRoomId)
                    .Select(l => new { l.MainManagerId, l.LabName })
                    .FirstOrDefaultAsync();

                if (labInfo.MainManagerId == null || labInfo == null) throw new NotFoundException("LabRoom", newBooking.LabRoomId.ToString());
                // --- GIAI ĐOẠN 1: LỌC SƠ BỘ (BROAD FILTER) TẠI DATABASE ---

                var creatorName = await dbContext.Users
                    .Where(u => u.Id == newBooking.CreatedById)
                    .Select(u => u.UserName) // Hoặc u.FullName nếu có
                    .FirstOrDefaultAsync() ?? "Người dùng";

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
                        (bs.Booking.Status == BookingStatus.Approved) &&
                        (bs.Status == BookingSlotStatus.Active)
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
                            throw new BadRequestException($"Xung đột ngày {conflict.Date}...");
                        }
                    }

                    // === QUYẾT ĐỊNH: KHÔNG XÓA CŨ - KHÔNG THÊM MỚI ===

                    // 1. Serialize danh sách slot mong muốn lại
                    newBooking.PendingSlotsJson = JsonSerializer.Serialize(newBooking.Slots);

                    // 2. Xóa sạch list Slots để EF Core KHÔNG insert vào bảng BookingSlot
                    //newBooking.Slots = null; // <--- KEY POINT

                    // 3. Set trạng thái Pending
                    newBooking.Status = BookingStatus.Pending;
                }
                else
                {
                    // Nếu KHÔNG có xung đột -> Insert thẳng vào BookingSlot luôn (nếu logic cho phép tự duyệt)
                    // Hoặc nếu quy trình bắt buộc duyệt -> Cũng làm y chang như trên (lưu JSON, Slots=null).

                    // Giả sử Priority Booking luôn cần duyệt:
                    newBooking.PendingSlotsJson = JsonSerializer.Serialize(newBooking.Slots);
                    //newBooking.Slots = null;
                    newBooking.Status = BookingStatus.Pending;
                }

                // --- GIAI ĐOẠN 4: LƯU MỚI ---

                // EF Core thông minh sẽ tự lưu Booking -> tự lưu BookingSlots -> tự lưu ExternalEquipments
                dbContext.Bookings.Add(newBooking);

                // [1] THÔNG BÁO CHO MANAGER (QUAN TRỌNG NHẤT)
                // Báo cho Manager biết có việc cần làm
                var mgrTitle = hasConflict ? "⚡ Có đơn ưu tiên cần xử lý" : "📅 Có đơn đặt phòng mới";
                var mgrBody = hasConflict
                    ? $"Đơn '{newBooking.Title}' tại '{labInfo.LabName}' đang trùng lịch và cần quyền ưu tiên."
                    : $"{creatorName} vừa đặt '{newBooking.Title}' tại '{labInfo.LabName}'. Vui lòng kiểm tra và duyệt.";

                var (_, mgrPush) = notificationRepo.PrepareNotification(
                    labInfo.MainManagerId, // Gửi về Manager
                    mgrTitle,
                    mgrBody,
                    "MANAGER_NEW_BOOKING",
                    new { bookingId = newBooking.Id, isPriority = hasConflict }
                );
                pushQueue.Add(mgrPush);

                await dbContext.SaveChangesAsync();

                // Commit Transaction: Chốt đơn!
                await transaction.CommitAsync();
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

                await dbContext.Entry(newBooking).Reference(b => b.LabRoom).LoadAsync();
                notificationRepo.RunPushNotificationTask(pushQueue);
                return newBooking;
            }
            catch
            {
                // Có bất kỳ lỗi gì -> Hoàn tác mọi thứ (kể cả việc xóa slot cũ)
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<(Booking Booking, bool HasPendingRequest)>> GetBookingsWithChangeStatusAsync(Guid userId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var query = dbContext.Bookings
                .AsNoTracking()
                .Include(b => b.LabRoom)

                // 🔥 [QUAN TRỌNG NHẤT]: Filtered Include
                // Dòng này bảo EF Core: "Chỉ nạp những slot nào Active và chưa qua ngày vào list Slots của Booking thôi"
                .Include(b => b.Slots
                    .Where(s => s.Status == BookingSlotStatus.Active && s.Date >= today)
                    .OrderBy(s => s.Date).ThenBy(s => s.Slot.StartTime)
                )
                .ThenInclude(s => s.Slot) // Load thêm thông tin ca giờ

                .Where(b =>
                    b.CreatedById == userId &&
                    b.Status == BookingStatus.Approved &&
                    // Điều kiện này để lọc Booking: Booking phải CÓ ít nhất 1 slot thỏa mãn mới lấy lên
                    b.Slots.Any(s => s.Date >= today && s.Status == BookingSlotStatus.Active)
                )
                .Select(b => new
                {
                    Booking = b,
                    // Check xem có ChangeRequest nào đang Pending không
                    HasPending = dbContext.BookingChangeRequests
                        .Any(cr => cr.BookingId == b.Id && cr.Status == BookingChangeRequestStatus.Pending)
                })
                .OrderByDescending(x => x.Booking.CreatedAt);

            var result = await query.ToListAsync();

            return result.Select(x => (x.Booking, x.HasPending)).ToList();
        }

        public async Task<Booking?> GetBookingDetailsAsync(Guid id)
        {
            return await dbContext.Bookings
                .AsNoTracking() // 1. Tối ưu hiệu năng vì chỉ đọc dữ liệu
                .Include(b => b.LabRoom)

                // 2. [QUAN TRỌNG] Chỉ lấy các Slot đang Active, sắp xếp theo ngày
                .Include(b => b.Slots
                    .Where(s => s.Status == BookingSlotStatus.Active)
                    .OrderBy(s => s.Date).ThenBy(s => s.Slot.StartTime)
                )
                .ThenInclude(s => s.Slot) // Load thêm thông tin Ca (nếu cần hiển thị giờ)

                .Include(b => b.Course)
                .Include(b => b.Project)
                .Include(b => b.BookingPriorityDetail)
                .Include(b => b.ExternalEquipments)
                .Include(b => b.OutSideGuests)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetPendingBookingsAsync(Guid? userId)
        {
            var query = dbContext.Bookings
                .Include(b => b.LabRoom)             // Lấy tên phòng
                .Include(b => b.Slots)               // Lấy các slot đã chọn
                .ThenInclude(s => s.Slot)            // Lấy chi tiết giờ (Ca 1: 7h-9h...)
                .Include(b => b.BookingPriorityDetail) // Lấy lý do ưu tiên (quan trọng để duyệt)
                .Include(b => b.Project)             // Lấy tên dự án
                .Include(b => b.Course)              // Lấy tên môn học
                .Include(b => b.ExternalEquipments)
                .Include(b => b.OutSideGuests)       //.Include(b => b.CreatedBy)         // (Optional) Nếu bạn có relationship với bảng User để hiện tên người đặt
                .Where(b => b.Status == BookingStatus.Pending);

            // Nếu có truyền LabId thì lọc, không thì lấy hết
            if (userId.HasValue)
            {
                query = query.Where(b => b.LabRoom.MainManagerId == userId);
            }

            // Sắp xếp: Đơn ưu tiên (VIP) lên đầu, hoặc đơn mới nhất lên đầu
            return await query
                .OrderByDescending(b => b.Priority) // Đơn Priority (1) lên trước Standard (2) (Lưu ý: Check lại Enum của bạn, số nào nhỏ hơn hay lớn hơn là VIP)
                .ThenBy(b => b.CreatedAt)           // Cùng mức ưu tiên thì đơn nào đến trước xử trước
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdWithSlotsAsync(Guid id)
        {
            return await dbContext.Bookings
                .Include(b => b.Slots)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<BookingSlot>> GetConflictingSlotsAsync(Guid labRoomId, List<BookingSlot> requestedSlots, Guid? excludeBookingId = null)
        {
            try
            {
                // 1. Validate
                if (requestedSlots == null || !requestedSlots.Any()) return new List<BookingSlot>();

                var dates = requestedSlots.Select(s => s.Date).Distinct().ToList();
                var slotIds = requestedSlots.Select(s => s.SlotId).Distinct().ToList();

                // 2. Prepare Data for DB Query (Fix PostgreSQL UTC)
                var minDateRaw = dates.Min().ToDateTime(TimeOnly.MinValue);
                var minDate = DateTime.SpecifyKind(minDateRaw, DateTimeKind.Utc);

                var maxDateRaw = dates.Max().ToDateTime(TimeOnly.MaxValue);
                var maxDate = DateTime.SpecifyKind(maxDateRaw, DateTimeKind.Utc);

                // =========================================================
                // [MỚI] PHẦN 0: CHECK SLOT TRONG QUÁ KHỨ (PAST TIME)
                // =========================================================

                // A. Cần lấy Master Data của Slot NGAY TỪ ĐẦU để biết giờ bắt đầu
                var slotMasterData = await dbContext.Slots
                    .AsNoTracking()
                    .Where(s => slotIds.Contains(s.Id))
                    .ToDictionaryAsync(s => s.Id);

                // B. Lấy giờ hiện tại ở Việt Nam
                var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowInVietnam = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTimeZone);

                var pastConflicts = new List<BookingSlot>();

                foreach (var reqSlot in requestedSlots)
                {
                    if (!slotMasterData.TryGetValue(reqSlot.SlotId, out var masterSlot)) continue;

                    // Tính thời gian bắt đầu của slot: Ngày (từ request) + Giờ (từ DB)
                    var slotStartTime = reqSlot.Date.ToDateTime(masterSlot.StartTime);

                    // So sánh: Nếu thời gian bắt đầu < Thời gian hiện tại -> Lỗi
                    if (slotStartTime < nowInVietnam)
                    {
                        pastConflicts.Add(new BookingSlot
                        {
                            Date = reqSlot.Date,
                            SlotId = reqSlot.SlotId,
                            // Bạn nên thêm 1 giá trị enum mới như 'PastTime' hoặc dùng tạm 'Locked'
                            Reason = UnavailableReason.PastTime // Hoặc tạo UnavailableReason.PastTime
                        });
                    }
                }

                // Nếu có slot trong quá khứ -> Return ngay lập tức, khỏi check cái khác
                if (pastConflicts.Any()) return pastConflicts;


                // =========================================================
                // PHẦN 1: CHECK TRÙNG VỚI BOOKING KHÁC
                // =========================================================
                var bookedConflicts = await dbContext.BookingSlots
                    .AsNoTracking()
                    .Include(bs => bs.Booking)
                    .Where(bs =>
                        bs.Booking.LabRoomId == labRoomId &&
                        bs.Status == BookingSlotStatus.Active &&
                        dates.Contains(bs.Date) &&
                        slotIds.Contains(bs.SlotId) &&
                        (excludeBookingId == null || bs.BookingId != excludeBookingId) &&
                        bs.Booking.Status == BookingStatus.Approved
                    )
                    .ToListAsync();

                var realConflicts = bookedConflicts.Where(dbSlot =>
                    requestedSlots.Any(req => req.Date == dbSlot.Date && req.SlotId == dbSlot.SlotId)
                ).ToList();

                if (realConflicts.Any()) return realConflicts;


                // =========================================================
                // PHẦN 2: CHECK TRÙNG LỊCH BẢO TRÌ
                // =========================================================

                var maintenanceSchedules = await dbContext.RoomMaintainSchedules
                    .AsNoTracking()
                    .Where(m =>
                        m.LabRoomId == labRoomId &&
                        m.EndTime > minDate &&
                        m.StartTime < maxDate &&
                        m.RoomMaintainStatus == RoomMaintainStatus.NotYet
                    ).ToListAsync();

                if (!maintenanceSchedules.Any()) return new List<BookingSlot>();

                // (Đã lấy slotMasterData ở trên rồi, không cần query lại nữa)

                foreach (var reqSlot in requestedSlots)
                {
                    if (!slotMasterData.TryGetValue(reqSlot.SlotId, out var masterSlot)) continue;

                    var localStart = reqSlot.Date.ToDateTime(masterSlot.StartTime);
                    var localEnd = reqSlot.Date.ToDateTime(masterSlot.EndTime);

                    var utcSlotStart = TimeZoneInfo.ConvertTime(localStart, localTimeZone, TimeZoneInfo.Utc);
                    var utcSlotEnd = TimeZoneInfo.ConvertTime(localEnd, localTimeZone, TimeZoneInfo.Utc);

                    var isUnderMaintenance = maintenanceSchedules.Any(m =>
                        utcSlotStart < m.EndTime && utcSlotEnd > m.StartTime
                    );

                    if (isUnderMaintenance)
                    {
                        realConflicts.Add(new BookingSlot
                        {
                            Date = reqSlot.Date,
                            SlotId = reqSlot.SlotId,
                            Reason = UnavailableReason.Maintenance
                        });
                    }
                }

                return realConflicts;
            }
            catch (Exception ex)
            {
                // Log error here
                throw new Exception("Có lỗi xảy ra.");
            }
        }

        // --- LOGIC CỐT LÕI NẰM Ở ĐÂY ---
        public async Task ApproveBookingWithOverrideLogicAsync(Booking booking)
        {
            var pushQueue = new List<PushNotificationData>();

            // 1. Tìm slot bị trùng
            var requestedSlots = booking.Slots.ToList();

            // Đảm bảo Include Booking để check Priority
            var conflicts = await GetConflictingSlotsAsync(booking.LabRoomId, requestedSlots, booking.Id);

            // [CHECK 1] Chặn slot quá khứ
            if (conflicts.Any(c => c.Reason == UnavailableReason.PastTime))
            {
                if (dbContext.Entry(booking).State == EntityState.Detached) dbContext.Bookings.Attach(booking);
                booking.Status = BookingStatus.Rejected;
                foreach (var s in booking.Slots) s.Status = BookingSlotStatus.Cancelled;

                var (_, rejectPush) = notificationRepo.PrepareNotification(
                    booking.CreatedById, "⛔ Đơn bị từ chối", "Lý do: Quá khứ...", "BOOKING_REJECTED",
                    new { bookingId = booking.Id, reason = "PastTime" });
                pushQueue.Add(rejectPush);

                await dbContext.SaveChangesAsync();
                notificationRepo.RunPushNotificationTask(pushQueue);
                throw new BadRequestException("Đơn đặt lịch đã bị TỪ CHỐI TỰ ĐỘNG vì chứa khung giờ trong quá khứ.");
            }

            // Loại bỏ chính nó
            conflicts = conflicts.Where(c => c.BookingId != booking.Id).ToList();
            string BuildConflictMessage(List<BookingSlot> conflictSlots)
            {
                var details = conflictSlots
                    .GroupBy(c => c.Date)
                    .OrderBy(g => g.Key)
                    .Select(g =>
                    {
                        var dateStr = g.Key.ToString("dd/MM");
                        // Lấy tên slot (ví dụ: Slot 1, Slot 2). Nếu null thì lấy giờ bắt đầu.
                        var slotNames = string.Join(", ", g.Select(s => s.Slot?.Label ?? s.SlotId.ToString().Substring(0, 4)).Distinct());
                        return $"{dateStr} ({slotNames})";
                    });

                return string.Join("; ", details);
            }
            // =====================================================================
            // CASE A: PRIORITY (GHI ĐÈ)
            // =====================================================================
            if (booking.Type == BookingType.UniversityEvent)
            {
                if (conflicts.Any())
                {
                    // --- [LOGIC MỚI] PHÂN LOẠI PRIORITY ---

                    // 1. Check xem có đụng "Hàng cứng" không (Priority <= Mình)
                    // Ví dụ: Mình là 1. Gặp thằng 0 (Bảo trì) hoặc 1 (Event khác) -> CHẶN
                    var blockingConflicts = conflicts
                        .Where(c => c.Booking != null && (int)c.Booking.Priority <= (int)booking.Priority)
                        .ToList();

                    if (blockingConflicts.Any())
                    {
                        // 👇 BÁO LỖI CHI TIẾT TẠI ĐÂY
                        var msg = BuildConflictMessage(blockingConflicts);
                        throw new BadRequestException($"Không thể duyệt vì vướng lịch Ưu tiên/Bảo trì tại: {msg}.");
                    }

                    // 2. Lọc ra những "Nạn nhân" (Priority > Mình)
                    // Ví dụ: Mình là 1. Gặp thằng 2 (Học/Dự án) -> GHI ĐÈ
                    var victimConflicts = conflicts
                        .Where(c => c.Booking != null && (int)c.Booking.Priority > (int)booking.Priority)
                        .ToList();

                    if (victimConflicts.Any())
                    {
                        var victimGroups = victimConflicts.GroupBy(c => c.BookingId);

                        foreach (var group in victimGroups)
                        {
                            var lostSlots = group.ToList();
                            var victimBooking = lostSlots.First().Booking;
                            if (victimBooking == null) continue;

                            // a. Đánh dấu slot cũ là Overridden
                            foreach (var slot in lostSlots)
                            {
                                slot.Booking = null;
                                if (dbContext.Entry(slot).State == EntityState.Detached) dbContext.BookingSlots.Attach(slot);

                                slot.Status = BookingSlotStatus.Overridden;
                                slot.OverriddenByBookingId = booking.Id;
                                dbContext.Entry(slot).State = EntityState.Modified;
                            }

                            // b. Tạo Consent Request
                            var lostSlotsDetail = lostSlots.Select(s => new { BookingSlotId = s.Id, Date = s.Date, SlotId = s.SlotId }).ToList();
                            var consentRequest = new BookingConsentRequest
                            {
                                Id = Guid.NewGuid(),
                                BookingId = victimBooking.Id,
                                CreatedById = victimBooking.CreatedById,
                                PriorityBookingId = booking.Id,
                                OverriddenSlotIdsJson = JsonSerializer.Serialize(lostSlotsDetail),
                                Status = ConsentStatus.Pending,
                                CreatedAt = DateTime.UtcNow
                            };
                            dbContext.Set<BookingConsentRequest>().Add(consentRequest);

                            // c. Thông báo Nạn nhân
                            var conflictDates = string.Join(", ", lostSlots.Select(s => s.Date.ToString("dd/MM")).Distinct());
                            var overridePayload = new
                            {
                                type = "OVERRIDE_CONSENT",
                                consentRequestId = consentRequest.Id,
                                bookingTitle = victimBooking.Title,
                                action = "resolve_override",
                                consentStatus = "Pending"
                            };
                            var (_, victimPush) = notificationRepo.PrepareNotification(
                                victimBooking.CreatedById,
                                "⚠️ Thay đổi lịch đặt phòng",
                                $"Đơn '{victimBooking.Title}' bị trùng lịch ngày {conflictDates} do sự kiện ưu tiên.",
                                "OVERRIDE_CONSENT",
                                overridePayload
                            );
                            pushQueue.Add(victimPush);
                        }
                    }
                }
            }
            // =====================================================================
            // CASE B: NORMAL (CHẶN HẾT)
            // =====================================================================
            else
            {
                if (conflicts.Any())
                    throw new BadRequestException("Không thể duyệt: Đã vướng lịch với đơn khác.");
            }

            // 2. DUYỆT ĐƠN MỚI
            if (dbContext.Entry(booking).State == EntityState.Detached) dbContext.Bookings.Attach(booking);
            booking.Status = BookingStatus.Approved;
            booking.ApprovedAt = DateTime.UtcNow;
            booking.QrCodeString = GenerateBookingCode(); // Tạo mã QR mới khi duyệt
            foreach (var slot in booking.Slots) slot.Status = BookingSlotStatus.Active;

            // 3. THÔNG BÁO CHỦ ĐƠN
            var (_, successPush) = notificationRepo.PrepareNotification(
                booking.CreatedById,
                "✅ Đơn đặt phòng đã được duyệt",
                $"Đơn '{booking.Title}' của bạn đã được quản lý chấp thuận.",
                "BOOKING_APPROVED",
                new { bookingId = booking.Id }
            );
            pushQueue.Add(successPush);

            // 4. LƯU & PUSH
            await dbContext.SaveChangesAsync();
            notificationRepo.RunPushNotificationTask(pushQueue);
        }

        public async Task<List<Guid>> GetBookedLabIdsAsync(DateOnly date, Guid slotId, CancellationToken ct)
        {// 1. Lấy ID phòng có Booking Active
            try
            {
                var bookedLabs = await dbContext.BookingSlots
                .AsNoTracking()
                .Where(bs => bs.Date == date
                          && bs.SlotId == slotId
                          && bs.Status == BookingSlotStatus.Active
                          && bs.Booking.Status == BookingStatus.Approved)
                .Select(bs => bs.Booking.LabRoomId)
                .ToListAsync(ct);

                // 2. Lấy ID phòng có Lịch bảo trì (Maintenance)
                var slotTemplate = await dbContext.Slots.FindAsync(new object[] { slotId }, ct);

                // Nếu không tìm thấy slot template thì coi như chỉ có booking bận
                if (slotTemplate == null) return bookedLabs.Distinct().ToList();

                var checkStart = DateTime.SpecifyKind(date.ToDateTime(slotTemplate.StartTime), DateTimeKind.Utc);
                var checkEnd = DateTime.SpecifyKind(date.ToDateTime(slotTemplate.EndTime), DateTimeKind.Utc);

                var maintenanceLabs = await dbContext.RoomMaintainSchedules
                    .AsNoTracking()
                    .Where(m =>
                        m.StartTime < checkEnd &&
                        m.EndTime > checkStart &&
                        m.RoomMaintainStatus == RoomMaintainStatus.NotYet
                    )
                    .Select(m => m.LabRoomId)
                    .ToListAsync(ct);

                // 3. Gộp lại và loại bỏ trùng lặp
                return bookedLabs.Concat(maintenanceLabs).Distinct().ToList();
            }
            catch (Exception ex)
            {
                // Ghi log nếu bạn có logger
                // _logger.LogError(ex, "Error at GetBookedLabIdsAsync");
                throw; // Quan trọng: giữ stack trace
            }
        }
        private string GenerateBookingCode(int length = 8)
        {
            // Tập hợp các ký tự cho phép (bỏ các ký tự dễ gây nhầm lẫn như I, O nếu muốn)
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<List<Booking>> GetHistoryByUserIdAsync(Guid userId)
        {
            return await dbContext.Bookings
                .Include(b => b.LabRoom)
                .Include(b => b.Slots.Where(s => s.Status == BookingSlotStatus.Active))
                    .ThenInclude(s => s.Slot) // Để lấy tên ca (Ca 1, Ca 2...)
                .Include(b => b.BookingPriorityDetail)
                .Include(b => b.Project)
                .Include(b => b.Course)
                .Where(b => b.CreatedById == userId) // Chỉ lấy của chính mình
                .OrderByDescending(b => b.CreatedAt) // Mới nhất lên đầu
                .ToListAsync();
        }

        public Task<bool> CheckBookingIsBelongToThisManager(Guid bookingId, Guid managerId)
        {
            return dbContext.Bookings
                .AnyAsync(b => b.Id == bookingId && b.LabRoom.MainManagerId == managerId);
        }

        public async Task RejectBookingAsync(Guid bookingId, Guid managerId, string? reason = null)
        {
            var pushQueue = new List<PushNotificationData>();
            // 1. Tìm Booking + Include Slots
            var booking = await dbContext.Bookings
                .Include(b => b.Slots)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                throw new BadRequestException("Không tìm thấy đơn đặt.");

            if (booking.Status != BookingStatus.Pending)
                throw new BadRequestException("Chỉ có thể từ chối đơn đang ở trạng thái chờ.");

            // 2. Update trạng thái Booking
            booking.Status = BookingStatus.Rejected;
            // booking.RejectedById = managerId; // (Optional) Nếu có trường này để lưu vết

            // 3. Hủy các Slot đang giữ chỗ (Để nhả phòng ra cho người khác)
            foreach (var slot in booking.Slots)
            {
                slot.Status = BookingSlotStatus.Cancelled;
                //kiểm tra chỗ này
            }

            // 4. [DÙNG REPO] Tạo thông báo cho User
            var message = $"Đơn '{booking.Title}' của bạn đã bị từ chối.";
            if (!string.IsNullOrEmpty(reason))
            {
                message += $" Lý do: {reason}";
            }

            var (_, pushData) = notificationRepo.PrepareNotification(
                booking.CreatedById,
                "⛔ Đơn đặt phòng bị từ chối",
                message,
                "BOOKING_REJECTED",
                new { bookingId = booking.Id, reason = reason }
            );
            pushQueue.Add(pushData);

            // 4. Save
            await dbContext.SaveChangesAsync();
            notificationRepo.RunPushNotificationTask(pushQueue);
        }

        //private async Task NotifyVictimAsync(Guid victimUserId, Guid consentRequestId, string bookingTitle, string customBody)
        //{
        //    try
        //    {
        //        // 1. Chuẩn bị nội dung
        //        var notiTitle = "⚠️ Thay đổi lịch đặt phòng";
        //        // Sử dụng nội dung chi tiết (đã có ngày tháng) được truyền vào
        //        var notiBody = customBody;

        //        // 2. Tạo Data Payload (Để Frontend xử lý điều hướng khi bấm vào thông báo)
        //        var dataPayloadObj = new
        //        {
        //            type = "OVERRIDE_CONSENT",       // Key quan trọng để FE biết chuyển hướng sang màn hình xử lý
        //            consentRequestId = consentRequestId, // ID để FE gọi API lấy chi tiết slot bị mất
        //            bookingTitle = bookingTitle,
        //            action = "resolve_override"
        //        };

        //        var jsonPayload = JsonSerializer.Serialize(dataPayloadObj);

        //        // 3. LƯU VÀO DATABASE (Để User xem lại trong mục "Thông báo" của App)
        //        var notification = new Notification
        //        {
        //            Id = Guid.NewGuid(),
        //            UserId = victimUserId,
        //            Title = notiTitle,
        //            Message = notiBody,
        //            IsRead = false,
        //            CreatedAt = DateTime.UtcNow, // Nhớ cấu hình Npgsql legacy timestamp nếu chưa
        //            DataPayload = jsonPayload,
        //            // Type = NotificationType.System // Nếu bạn có Enum phân loại
        //        };

        //        // Vì hàm này được gọi SAU KHI transaction chính đã commit, 
        //        // nên ta Add và SaveChanges ngay lập tức cho Notification.
        //        dbContext.Notifications.Add(notification);
        //        await dbContext.SaveChangesAsync();

        //        // 4. GỬI PUSH NOTIFICATION (Đến điện thoại)
        //        // Lấy danh sách Device Token của User
        //        // (Giả sử bạn đã inject _userDeviceRepository vào Constructor của Repository này)
        //        var tokens = await userDeviceRepository.GetTokensByUserIdAsync(victimUserId, CancellationToken.None);

        //        if (tokens != null && tokens.Any())
        //        {
        //            // Gọi Service gửi Push (Firebase/Expo)
        //            // (Giả sử bạn đã inject _notificationService)
        //            await notificationService.SendPushNotificationAsync(tokens, notiTitle, notiBody, dataPayloadObj);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Quan trọng: Bắt lỗi để không làm crash luồng chính
        //        // (Ví dụ: Lỗi mạng khi gọi Firebase, hoặc User chưa có Token)
        //        Console.WriteLine($"[NotifyVictimAsync] Gửi thông báo thất bại: {ex.Message}");

        //        // _logger.LogError(ex, "..."); // Nếu có Logger
        //    }
        //}        

        public async Task<List<Booking>> GetApprovedHistoryByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await dbContext.Bookings
                .AsNoTracking() // Read-only nên dùng NoTracking cho nhanh
                .Include(b => b.LabRoom)

                // [QUAN TRỌNG] Filtered Include: Chỉ lấy các slot đang Active
                // Giúp giảm tải dữ liệu rác (những slot đã hủy hoặc bị đè)
                .Include(b => b.Slots.Where(s => s.Status == BookingSlotStatus.Active))

                .Where(b =>
                    b.CreatedById == userId &&          // Của chính mình
                    b.Status == BookingStatus.Approved  // Đã được duyệt
                )
                .OrderByDescending(b => b.CreatedAt)    // Mới nhất lên đầu
                .ToListAsync(cancellationToken);
        }
    }
}
