internal class RoomMaintainScheduleRepository(LabBookingDbContext dbContext, INotificationRepository notificationRepo) : IRoomMaintainScheduleRepository
{
    public async Task<IEnumerable<RoomMaintainSchedule>> GetOverlappingSchedulesAsync(
        Guid labRoomId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        // --- SỬA LỖI Ở ĐÂY ---

        // 1. Tạo ngày bắt đầu (T2 00:00:00) ở múi giờ Local của server
        var localQueryStartDate = startDate.ToDateTime(TimeOnly.MinValue);

        // 2. Tạo ngày kết thúc (T2 tuần sau 00:00:00) ở múi giờ Local
        var localQueryEndDate = endDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

        // 3. Chuyển đổi cả hai sang UTC để query
        // (Giả sử server chạy ở UTC+7, 00:00 Local -> 17:00 (ngày hôm trước) UTC)
        var queryStartDateUtc = localQueryStartDate.ToUniversalTime();
        var queryEndDateUtc = localQueryEndDate.ToUniversalTime();

        // --- KẾT THÚC SỬA ---

        // Logic tìm chồng chéo (overlap) giờ đã đúng múi giờ
        // (Schedule.StartTime < queryEndDateUtc) AND (Schedule.EndTime > queryStartDateUtc)

        var schedules = await dbContext.RoomMaintainSchedules
            .Where(m =>
                m.LabRoomId == labRoomId &&
                m.StartTime < queryEndDateUtc &&  // So sánh (timestamptz < Utc)
                m.EndTime > queryStartDateUtc &&  // So sánh (timestamptz > Utc)
                m.RoomMaintainStatus == RoomMaintainStatus.NotYet
            )
            .ToListAsync(cancellationToken);

        return schedules;
    }

    public async Task<Guid> Create(RoomMaintainSchedule entity, CancellationToken cancellationToken = default)
    {
        // Giả định DbContext của bạn có DbSet tên là RoomMaintainSchedules
        dbContext.RoomMaintainSchedules.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id; // Trả về Id giống hệt LabRoomRepository
    }

    // 👇 HÀM MỚI: TẠO BẢO TRÌ VÀ XỬ LÝ GHI ĐÈ (OVERRIDE)
    public async Task<Guid> CreateWithOverrideLogicAsync(RoomMaintainSchedule schedule, CancellationToken cancellationToken)
    {
        var pushQueue = new List<PushNotificationData>();

        // 1. TÌM CÁC SLOT BỊ ẢNH HƯỞNG
        // Vì BookingSlot chỉ lưu Date và SlotId, ta cần join với Slot Template để biết giờ bắt đầu/kết thúc cụ thể

        // A. Lấy tất cả slot active của phòng này trong khoảng ngày bảo trì
        var candidateSlots = await dbContext.BookingSlots
            .Include(s => s.Booking)
            .Include(s => s.Slot) // Include Template giờ
            .Where(s =>
                s.Booking.LabRoomId == schedule.LabRoomId &&
                s.Status == BookingSlotStatus.Active &&
                s.Date >= DateOnly.FromDateTime(schedule.StartTime) &&
                s.Date <= DateOnly.FromDateTime(schedule.EndTime)
            )
            .ToListAsync(cancellationToken);

        // B. Lọc chính xác theo giờ (Time Overlap)
        // Công thức trùng: (StartA < EndB) && (EndA > StartB)
        var conflictingSlots = candidateSlots.Where(s =>
        {
            var slotStart = s.Date.ToDateTime(s.Slot.StartTime);
            var slotEnd = s.Date.ToDateTime(s.Slot.EndTime);

            return slotStart < schedule.EndTime && slotEnd > schedule.StartTime;
        }).ToList();

        // 2. XỬ LÝ GHI ĐÈ (GROUP BY BOOKING)
        if (conflictingSlots.Any())
        {
            var victimGroups = conflictingSlots.GroupBy(s => s.BookingId);

            foreach (var group in victimGroups)
            {
                var lostSlots = group.ToList();
                var victimBooking = lostSlots.First().Booking;

                // a. Cập nhật trạng thái slot cũ -> Overridden
                foreach (var slot in lostSlots)
                {
                    slot.Status = BookingSlotStatus.Overridden;
                    // slot.OverriddenByBookingId = null; // Không phải do booking đè
                    // slot.OverriddenByMaintainScheduleId = schedule.Id; // (Nếu bạn đã thêm cột này vào BookingSlot)

                    dbContext.Entry(slot).State = EntityState.Modified;
                }

                // b. Tạo BookingConsentRequest (Yêu cầu sự đồng ý/chọn lại)
                var lostSlotsDetail = lostSlots.Select(s => new { BookingSlotId = s.Id, Date = s.Date, SlotId = s.SlotId }).ToList();

                var consentRequest = new BookingConsentRequest
                {
                    Id = Guid.NewGuid(),
                    BookingId = victimBooking.Id,
                    CreatedById = victimBooking.CreatedById,

                    // PriorityBookingId = null, // Vì đây là do bảo trì
                    RoomMaintainScheduleId = schedule.Id, // 👈 BẠN CẦN THÊM CỘT NÀY VÀO DB NẾU MUỐN TRACKING KỸ

                    OverriddenSlotIdsJson = JsonSerializer.Serialize(lostSlotsDetail),
                    Status = ConsentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                dbContext.Set<BookingConsentRequest>().Add(consentRequest);

                // c. [DÙNG REPO] Tạo thông báo cho Nạn Nhân
                var conflictDates = string.Join(", ", lostSlots.Select(s => s.Date.ToString("dd/MM")).Distinct());

                var overridePayload = new
                {
                    type = "OVERRIDE_CONSENT",
                    consentRequestId = consentRequest.Id,
                    bookingTitle = victimBooking.Title,
                    action = "resolve_override",
                    consentStatus = "Pending",
                    reason = "Maintenance" // Đánh dấu là do bảo trì
                };

                var (_, victimPush) = notificationRepo.PrepareNotification(
                    victimBooking.CreatedById,
                    "🛠️ Lịch đặt phòng bị hủy do bảo trì",
                    $"Đơn '{victimBooking.Title}' bị ảnh hưởng bởi lịch bảo trì phòng thí nghiệm vào ngày {conflictDates}. Vui lòng chọn lịch bù.",
                    "OVERRIDE_CONSENT",
                    overridePayload
                );
                pushQueue.Add(victimPush);
            }
        }

        // 3. LƯU LỊCH BẢO TRÌ & COMMIT DB
        schedule.Id = Guid.NewGuid(); // Đảm bảo ID mới
        if (schedule.RoomMaintainStatus == null) schedule.RoomMaintainStatus = RoomMaintainStatus.NotYet;

        dbContext.RoomMaintainSchedules.Add(schedule);

        await dbContext.SaveChangesAsync(cancellationToken);

        // 4. BẮN PUSH (Background)
        notificationRepo.RunPushNotificationTask(pushQueue);

        return schedule.Id;
    }

    public async Task Update(RoomMaintainSchedule entity, CancellationToken cancellationToken = default)
    {
        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(RoomMaintainSchedule entity, CancellationToken cancellationToken = default)
    {
        dbContext.RoomMaintainSchedules.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task<RoomMaintainSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Giả định DbContext của bạn có DbSet<RoomMaintainSchedule> tên là RoomMaintainSchedules
        var schedule = await dbContext.RoomMaintainSchedules.FindAsync(new object[] { id }, cancellationToken);
        return schedule;
    }

    public async Task<(IEnumerable<RoomMaintainSchedule>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        RoomMaintainStatus? status, // Tham số lọc mới
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken = default)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        // 1. Query cơ sở
        var baseQuery = dbContext
            .RoomMaintainSchedules
            // Lọc theo SearchPhrase
            .Where(s => searchPhraseLower == null ||
                        (s.Description != null && s.Description.ToLower().Contains(searchPhraseLower)))

            // BỎ MỆNH ĐỀ .Where(s => labRoomId == null || s.LabRoomId == labRoomId)

            // 2. Lọc theo Status
            .Where(s => status == null || s.RoomMaintainStatus == status);

        // 3. Đếm tổng số lượng
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        // 4. Sắp xếp (giữ nguyên)
        if (sortBy != null)
        {
            // ... (logic sắp xếp giữ nguyên)
            var columnsSelector = new Dictionary<string, Expression<Func<RoomMaintainSchedule, object>>>
            {
                { nameof(RoomMaintainSchedule.StartTime), s => s.StartTime! },
                { nameof(RoomMaintainSchedule.EndTime), s => s.EndTime! },
                { nameof(RoomMaintainSchedule.RoomMaintainStatus), s => s.RoomMaintainStatus! }
            };

            if (columnsSelector.TryGetValue(sortBy, out var selectedColumn))
            {
                baseQuery = sortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }
        }
        else
        {
            baseQuery = baseQuery.OrderByDescending(s => s.StartTime);
        }

        // 5. Phân trang
        var schedules = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (schedules, totalCount);
    }

    public async Task<IEnumerable<RoomMaintainSchedule>> GetExpiredNotYetSchedulesAsync(CancellationToken cancellationToken = default)
    {
        // So sánh EndTime với thời gian hiện tại UTC
        var expiredSchedules = await dbContext.RoomMaintainSchedules
            .Where(s => s.EndTime < DateTime.UtcNow && s.RoomMaintainStatus == RoomMaintainStatus.NotYet)
            .ToListAsync(cancellationToken);

        return expiredSchedules;
    }

    public async Task UpdateRange(IEnumerable<RoomMaintainSchedule> schedules, CancellationToken cancellationToken = default)
    {
        foreach (var schedule in schedules)
        {
            // Set trạng thái sang Done
            schedule.RoomMaintainStatus = RoomMaintainStatus.Done;
            // Tùy chọn: Cập nhật thêm trường thời gian hoàn thành nếu có
        }

        // Đánh dấu Entity là cần cập nhật (EF Core sẽ nhận biết)
        dbContext.RoomMaintainSchedules.UpdateRange(schedules);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
