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

    public async Task<RoomMaintainSchedule> Create(RoomMaintainSchedule entity, CancellationToken cancellationToken = default)
    {
        dbContext.RoomMaintainSchedules.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Entry(entity)
            .Reference(e => e.LabRoom)
            .LoadAsync(cancellationToken);
        return entity;
    }

    // 👇 HÀM MỚI: TẠO BẢO TRÌ VÀ XỬ LÝ GHI ĐÈ (OVERRIDE)
    public async Task<Guid> CreateWithOverrideLogicAsync(RoomMaintainSchedule schedule, CancellationToken cancellationToken)
    {
        var pushQueue = new List<PushNotificationData>();

        try
        {
            // =========================================================
            // 1. XỬ LÝ TIMEZONE & QUERY DATA
            // =========================================================

            // Lấy múi giờ VN để tính toán ngày cho chuẩn (Tránh vụ 6h sáng thành 11h đêm hôm trước)
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Convert giờ bảo trì (UTC) sang giờ VN
            var vnStart = TimeZoneInfo.ConvertTimeFromUtc(schedule.StartTime, vnTimeZone);
            var vnEnd = TimeZoneInfo.ConvertTimeFromUtc(schedule.EndTime, vnTimeZone);

            // Lấy ngày bắt đầu và kết thúc theo giờ VN (Để so khớp với Date trong BookingSlot)
            var dateStart = DateOnly.FromDateTime(vnStart);
            var dateEnd = DateOnly.FromDateTime(vnEnd);

            // [FIX LỖI INCLUDE]: Chuyển điều kiện Approved xuống Where
            var candidateSlots = await dbContext.BookingSlots
                .Include(s => s.Booking) // Chỉ Include Booking, không filter ở đây
                .Include(s => s.Slot)    // Include Template giờ
                .Where(s =>
                    s.Booking.LabRoomId == schedule.LabRoomId &&
                    s.Status == BookingSlotStatus.Active &&
                    // Lọc những booking đã Approved
                    s.Booking.Status == BookingStatus.Approved &&
                    // So sánh ngày dựa trên giờ VN đã convert
                    s.Date >= dateStart &&
                    s.Date <= dateEnd
                )
                .ToListAsync(cancellationToken);

            // =========================================================
            // 2. LỌC CHÍNH XÁC THEO GIỜ (IN MEMORY)
            // =========================================================
            var conflictingSlots = candidateSlots.Where(s =>
            {
                // Tái tạo thời gian thực của Slot (Theo giờ VN)
                // s.Date là ngày (VD: 10/12), s.Slot.StartTime là giờ (VD: 07:00)
                var slotStartLocal = s.Date.ToDateTime(s.Slot.StartTime);
                var slotEndLocal = s.Date.ToDateTime(s.Slot.EndTime);

                // Convert Slot sang UTC để so sánh với Schedule (Schedule luôn là UTC)
                // Hoặc Convert Schedule sang Local để so sánh (Ở trên đã có vnStart, vnEnd)
                // Cách nào cũng được, miễn là cùng hệ quy chiếu. Dưới đây dùng hệ VN (Local)

                // Logic trùng: (StartA < EndB) && (EndA > StartB)
                return slotStartLocal < vnEnd && slotEndLocal > vnStart;
            }).ToList();

            // =========================================================
            // 3. XỬ LÝ GHI ĐÈ (NẾU CÓ TRÙNG)
            // =========================================================
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
                        // slot.OverriddenByMaintainScheduleId = schedule.Id; // Nếu có cột này thì gán vào
                        dbContext.Entry(slot).State = EntityState.Modified;
                    }

                    // b. Tạo Consent Request
                    var lostSlotsDetail = lostSlots.Select(s => new { BookingSlotId = s.Id, Date = s.Date, SlotId = s.SlotId }).ToList();

                    var consentRequest = new BookingConsentRequest
                    {
                        Id = Guid.NewGuid(),
                        BookingId = victimBooking.Id,
                        CreatedById = victimBooking.CreatedById,
                        // PriorityBookingId để null vì đây là do bảo trì
                        OverriddenSlotIdsJson = JsonSerializer.Serialize(lostSlotsDetail),
                        Status = ConsentStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };
                    dbContext.Set<BookingConsentRequest>().Add(consentRequest);

                    // c. Tạo thông báo + Push
                    var conflictDates = string.Join(", ", lostSlots.Select(s => s.Date.ToString("dd/MM")).Distinct());

                    var overridePayload = new
                    {
                        type = "OVERRIDE_CONSENT",
                        consentRequestId = consentRequest.Id,
                        bookingTitle = victimBooking.Title,
                        action = "resolve_override",
                        consentStatus = "Pending",
                        reason = "Maintenance"
                    };

                    var (_, victimPush) = notificationRepo.PrepareNotification(
                        victimBooking.CreatedById,
                        "🛠️ Lịch đặt phòng bị hủy do bảo trì",
                        $"Đơn '{victimBooking.Title}' bị ảnh hưởng bởi lịch bảo trì vào ngày {conflictDates}. Vui lòng chọn lịch bù.",
                        "OVERRIDE_CONSENT",
                        overridePayload
                    );
                    pushQueue.Add(victimPush);
                }
            }

            // =========================================================
            // 4. LƯU LỊCH BẢO TRÌ & COMMIT
            // =========================================================
            schedule.Id = Guid.NewGuid();
            if (schedule.RoomMaintainStatus == null) schedule.RoomMaintainStatus = RoomMaintainStatus.NotYet;

            dbContext.RoomMaintainSchedules.Add(schedule);

            await dbContext.SaveChangesAsync(cancellationToken);

            // 5. BẮN PUSH
            notificationRepo.RunPushNotificationTask(pushQueue);

            return schedule.Id;
        }
        catch (Exception ex)
        {
            // Log lỗi tại đây (Nếu có ILogger)
            Console.WriteLine($"Error creating maintain schedule: {ex.Message}");
            throw; // Ném lỗi ra ngoài để Controller trả về 500
        }
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
        RoomMaintainStatus? status,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        Guid? managerId,
        CancellationToken cancellationToken = default)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = dbContext.RoomMaintainSchedules
            .Include(s => s.LabRoom)
            .AsQueryable();

        if (managerId.HasValue)
        {
            baseQuery = baseQuery.Where(s => s.LabRoom != null && s.LabRoom.MainManagerId == managerId);
        }

        if (!string.IsNullOrEmpty(searchPhraseLower))
        {
            baseQuery = baseQuery.Where(r =>
                r.Description != null && r.Description.ToLower().Contains(searchPhraseLower));
        }

        if (status.HasValue)
        {
            baseQuery = baseQuery.Where(s => s.RoomMaintainStatus == status);
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        if (sortBy != null)
        {
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
