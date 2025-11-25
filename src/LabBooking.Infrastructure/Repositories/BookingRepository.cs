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

                await dbContext.Entry(newBooking).Reference(b => b.LabRoom).LoadAsync();

                return newBooking;
            }
            catch
            {
                // Có bất kỳ lỗi gì -> Hoàn tác mọi thứ (kể cả việc xóa slot cũ)
                //await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<(Booking Booking, bool HasPendingRequest)>> GetBookingsWithChangeStatusAsync(Guid userId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var query = dbContext.Bookings
                .Include(b => b.LabRoom)
                .Include(b => b.Slots)
                .Where(b =>
                    b.CreatedById == userId &&
                    b.Status == BookingStatus.Approved &&
                    b.Slots.Any(s => s.Date >= today)
                )
                .Select(b => new
                {
                    Booking = b,
                    HasPending = dbContext.BookingChangeRequests.Any(cr => cr.BookingId == b.Id && cr.Status == BookingChangeRequestStatus.Pending)
                })
                .OrderByDescending(x => x.Booking.CreatedAt);

            var result = await query.ToListAsync();
            return result.Select(x => (x.Booking, x.HasPending)).ToList();
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

        public async Task<List<Booking>> GetPendingBookingsAsync(Guid? labId)
        {
            var query = dbContext.Bookings
                .Include(b => b.LabRoom)             // Lấy tên phòng
                .Include(b => b.Slots)               // Lấy các slot đã chọn
                .ThenInclude(s => s.Slot)            // Lấy chi tiết giờ (Ca 1: 7h-9h...)
                .Include(b => b.BookingPriorityDetail) // Lấy lý do ưu tiên (quan trọng để duyệt)
                .Include(b => b.Project)             // Lấy tên dự án
                .Include(b => b.Course)              // Lấy tên môn học
                                                     //.Include(b => b.CreatedBy)         // (Optional) Nếu bạn có relationship với bảng User để hiện tên người đặt
                .Where(b => b.Status == BookingStatus.Pending);

            // Nếu có truyền LabId thì lọc, không thì lấy hết
            if (labId.HasValue)
            {
                query = query.Where(b => b.LabRoomId == labId);
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
                            Reason = UnavailableReason.Booked // Hoặc tạo UnavailableReason.PastTime
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
                    .Where(bs =>
                        bs.Booking.LabRoomId == labRoomId &&
                        bs.Status == BookingSlotStatus.Active &&
                        dates.Contains(bs.Date) &&
                        slotIds.Contains(bs.SlotId) &&
                        (excludeBookingId == null || bs.BookingId != excludeBookingId)
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
            // 1. Tìm slot bị trùng
            var requestedSlots = booking.Slots.ToList();
            var conflicts = await GetConflictingSlotsAsync(booking.LabRoomId, requestedSlots, booking.Id);

            // Loại bỏ chính nó (đề phòng)
            conflicts = conflicts.Where(c => c.BookingId != booking.Id).ToList();

            // =====================================================================
            // CASE A: NẾU LÀ PRIORITY (UniversityEvent) -> ĐÈ & TẠO CONSENT
            // =====================================================================
            if (booking.Type == BookingType.UniversityEvent)
            {
                if (conflicts.Any())
                {
                    // Gom nhóm các nạn nhân
                    var victimGroups = conflicts.GroupBy(c => c.Booking);

                    foreach (var group in victimGroups)
                    {
                        var victimBooking = group.Key;
                        var lostSlots = group.ToList();

                        // a. Đánh dấu slot cũ là Overridden (Vô hiệu hóa)
                        foreach (var slot in lostSlots)
                        {
                            if (dbContext.Entry(slot).State == EntityState.Detached)
                            {
                                dbContext.BookingSlots.Attach(slot);
                            }

                            slot.Status = BookingSlotStatus.Overridden;
                            slot.OverriddenByBookingId = booking.Id;

                            dbContext.Entry(slot).State = EntityState.Modified;
                        }

                        // b. Tạo BookingConsentRequest cho User cũ
                        var consentRequest = new BookingConsentRequest
                        {
                            Id = Guid.NewGuid(),
                            BookingId = victimBooking.Id,
                            CreatedById = victimBooking.CreatedById,
                            PriorityBookingId = booking.Id,

                            // Serialize List Guid thành JSON
                            OverriddenSlotIdsJson = JsonSerializer.Serialize(lostSlots.Select(s => s.Id)),

                            Status = ConsentStatus.Pending,
                            CreatedAt = DateTime.UtcNow
                        };

                        dbContext.Set<BookingConsentRequest>().Add(consentRequest);
                    }
                }
            }
            // =====================================================================
            // CASE B: NẾU LÀ ĐƠN THƯỜNG -> CÓ TRÙNG LÀ CHẶN
            // =====================================================================
            else
            {
                if (conflicts.Any())
                    throw new InvalidOperationException("Không thể duyệt: Đã vướng lịch với đơn khác.");
            }
            if (dbContext.Entry(booking).State == EntityState.Detached)
            {
                dbContext.Bookings.Attach(booking);
            }

            // 2. DUYỆT ĐƠN MỚI (Thành công cho cả 2 trường hợp nếu qua được bước trên)
            booking.Status = BookingStatus.Approved;

            // Active các slot của đơn này lên
            foreach (var slot in booking.Slots)
            {
                slot.Status = BookingSlotStatus.Active;
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Guid>> GetBookedLabIdsAsync(DateOnly date, Guid slotId, CancellationToken ct)
        {
            return await dbContext.BookingSlots
                .Include(bs => bs.Booking)
                .Where(bs => bs.Date == date
                          && bs.SlotId == slotId
                          && bs.Booking.Status != BookingStatus.Cancelled
                          && bs.Booking.Status != BookingStatus.Rejected)
                .Select(bs => bs.Booking.LabRoomId)
                .Distinct()
                .ToListAsync(ct);
        }

        public async Task<List<Booking>> GetHistoryByUserIdAsync(Guid userId)
        {
            return await dbContext.Bookings
                .Include(b => b.LabRoom)
                .Include(b => b.Slots)
                    .ThenInclude(s => s.Slot) // Để lấy tên ca (Ca 1, Ca 2...)
                .Include(b => b.BookingPriorityDetail)
                .Include(b => b.Project)
                .Include(b => b.Course)
                .Where(b => b.CreatedById == userId) // Chỉ lấy của chính mình
                .OrderByDescending(b => b.CreatedAt) // Mới nhất lên đầu
                .ToListAsync();
        }
    }
}
