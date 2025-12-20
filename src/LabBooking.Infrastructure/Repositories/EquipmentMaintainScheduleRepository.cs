using LabBooking.Domain.Enums;
using LabBooking.Domain.NonEntities;

namespace LabBooking.Infrastructure.Repositories;

internal class EquipmentMaintainScheduleRepository(
    LabBookingDbContext dbContext,
    ILogger<EquipmentMaintainScheduleRepository> logger) : IEquipmentMaintainScheduleRepository
{
    public async Task CreateAsync(EquipmentMaintainSchedule schedule, CancellationToken token)
    {
        await dbContext.EquipmentMaintainSchedules.AddAsync(schedule, token);
        await dbContext.SaveChangesAsync(token);
    }

    public async Task<bool> IsOverlapAsync(Guid equipmentId, DateTime start, DateTime end, CancellationToken token)
    {
        var utcStart = start.ToUniversalTime();
        var utcEnd = end.ToUniversalTime();

        return await dbContext.EquipmentMaintenances
            .Include(x => x.Schedule)
            .AnyAsync(x =>
                x.EquipmentId == equipmentId &&
                x.Status != MaintenanceStatus.Done &&
                x.Schedule.StartTime < utcEnd && utcStart < x.Schedule.EndTime,
                token);
    }

    public async Task<string> ProcessAutomatedMaintenanceAsync(CancellationToken token)
    {
        var now = DateTime.UtcNow;
        var lookAheadTime = now.AddMinutes(5);
        int startedCount = 0;
        int endedCount = 0;

        // ---------------------------------------------------------
        // 1. LUỒNG BẮT ĐẦU (START)
        // ---------------------------------------------------------
        var runningSchedules = await dbContext.EquipmentMaintainSchedules
            .Include(s => s.Details)
            .ThenInclude(d => d.Equipment)
            .Where(s => s.StartTime <= lookAheadTime
                     && s.EndTime > now
                     && s.Status != MaintenanceStatus.Done)
            .ToListAsync(token);

        // Tạo một danh sách các ID thiết bị ĐANG BẬN để dùng cho bước 2
        // (Để tránh việc bước 2 trả nhầm thiết bị đang cần bảo trì về Available)
        var busyEquipmentIds = new HashSet<Guid>();

        foreach (var schedule in runningSchedules)
        {
            foreach (var detail in schedule.Details)
            {
                if (detail.Equipment != null)
                {
                    // Lưu lại ID thiết bị đang được xử lý bảo trì
                    busyEquipmentIds.Add(detail.EquipmentId);

                    // Logic chuyển trạng thái
                    if (detail.Equipment.Status != EquipmentStatus.Maintain)
                    {
                        detail.Equipment.Status = EquipmentStatus.Maintain;
                        detail.Equipment.IsAvailable = false;
                        startedCount++;
                    }
                }
            }
        }

        // ---------------------------------------------------------
        // 2. LUỒNG KẾT THÚC (END)
        // ---------------------------------------------------------
        var expiredSchedules = await dbContext.EquipmentMaintainSchedules
            .Include(s => s.Details)
            .ThenInclude(d => d.Equipment)
            .Where(s => s.EndTime <= now
                     && s.Status != MaintenanceStatus.Done)
            .ToListAsync(token);

        foreach (var schedule in expiredSchedules)
        {
            schedule.Status = MaintenanceStatus.Done;

            foreach (var detail in schedule.Details)
            {
                if (detail.Status != MaintenanceStatus.Done)
                {
                    detail.Status = MaintenanceStatus.Done;
                    detail.ResultNote = detail.ResultNote ?? "Auto-completed by System";
                }

                if (detail.Equipment != null)
                {
                    // RULE SỬA LỖI:
                    // Chỉ trả về Available NẾU thiết bị đó KHÔNG nằm trong danh sách đang bận (busyEquipmentIds)
                    bool isBusyInOtherSchedule = busyEquipmentIds.Contains(detail.EquipmentId);

                    if (!isBusyInOtherSchedule && detail.Equipment.Status == EquipmentStatus.Maintain)
                    {
                        detail.Equipment.Status = EquipmentStatus.Available;
                        detail.Equipment.IsAvailable = true;

                        // Force Update status thiết bị
                        dbContext.Entry(detail.Equipment).State = EntityState.Modified;

                        // ------------------------------------------------------------
                        // --- NEW LOGIC: TỰ ĐỘNG ĐÓNG SỰ CỐ LIÊN QUAN (AUTO-RESOLVE) ---
                        // ------------------------------------------------------------

                        // Tìm tất cả sự cố của máy này mà CHƯA ĐƯỢC XỬ LÝ
                        var relatedIncidents = await dbContext.Incidents
                            .Include(i => i.IncidentEquipments)
                                .ThenInclude(ie => ie.Equipment) // Include để check trạng thái các máy khác
                            .Where(i => !i.IsResolved &&
                                        i.IncidentEquipments.Any(ie => ie.EquipmentId == detail.EquipmentId))
                            .ToListAsync(token);

                        if (relatedIncidents.Any())
                        {
                            foreach (var incident in relatedIncidents)
                            {
                                // 2. CHECK KỸ: Liệu TẤT CẢ thiết bị trong Incident này đã OK chưa?
                                // (Ngoại trừ cái detail.EquipmentId này vì mình vừa set nó Available xong)

                                bool allFixed = incident.IncidentEquipments.All(ie =>
                                    ie.EquipmentId == detail.EquipmentId || // Là máy đang sửa -> Coi như OK
                                    (ie.Equipment != null && ie.Equipment.Status == EquipmentStatus.Available) // Các máy khác đã OK
                                );

                                if (allFixed)
                                {
                                    // Chỉ đóng khi tất cả máy trong sự cố đã ngon lành
                                    incident.IsResolved = true;
                                    incident.ResolvedAt = DateTime.UtcNow;
                                    dbContext.Entry(incident).State = EntityState.Modified;

                                    detail.ResultNote += $" [Auto-Close Incident #{incident.Id.ToString().Substring(0, 4)}]";
                                }
                                else
                                {
                                    // Nếu chưa sửa hết -> Vẫn để Incident mở, để Manager nhớ sửa nốt máy kia
                                    detail.ResultNote += $" [Fix part of Incident #{incident.Id.ToString().Substring(0, 4)}]";
                                }
                            }
                        }
                        // ============================================================
                    }
                }
            }
            endedCount++;
        }

        // SaveChanges 1 lần duy nhất
        if (startedCount > 0 || endedCount > 0)
        {
            await dbContext.SaveChangesAsync(token);
        }

        var anomalySchedules = await dbContext.EquipmentMaintainSchedules
        .Where(s => s.StartTime < DateTime.UtcNow.AddMinutes(-15) // Đã quá khứ 15p
                 && s.Status == MaintenanceStatus.NotYet)         // Mà chưa chạy?
        .ToListAsync(token);

        if (anomalySchedules.Any())
        {
            var ids = string.Join(", ", anomalySchedules.Select(s => s.Id));
            var errorMsg = $"CRITICAL ERROR: Phát hiện {anomalySchedules.Count} lịch bị bỏ quên! (IDs: {ids}). Kiểm tra ngay logic DateTime hoặc Server CronJob.";

            // 1. Log lỗi nghiêm trọng (Hiện đỏ trong console/file log)
            logger.LogError(errorMsg);

            // 2. (Nâng cao) Gửi thông báo về Telegram/Slack/Email cho Dev (Xem Cách 2)
            //await SendAlertToDevTeamAsync(errorMsg);
        }

        return $"Job Report: Đã chuyển {startedCount} thiết bị sang 'Maintain' | Đã hoàn tất {endedCount} lịch trình.";
    }

    public async Task<ScheduleConflictInfo?> GetConflictInfoAsync(Guid equipmentId, DateTime start, DateTime end, CancellationToken token)
    {
        var utcStart = start.ToUniversalTime();
        var utcEnd = end.ToUniversalTime();

        // Query lấy thông tin chi tiết của lịch đang trùng
        var conflict = await dbContext.EquipmentMaintenances
            .Include(x => x.Equipment)
                .ThenInclude(e => e.LabRoom)
            .Include(x => x.Schedule)
            .Where(x =>
                x.EquipmentId == equipmentId &&
                x.Status != MaintenanceStatus.Done &&
                x.Schedule.StartTime < utcEnd && utcStart < x.Schedule.EndTime)
            .Select(x => new ScheduleConflictInfo
            {
                EquipmentName = x.Equipment.EquipmentName,
                LabRoomName = x.Equipment.LabRoom.LabName ?? "Kho/Chưa phân phòng",
                StartTime = x.Schedule.StartTime,
                EndTime = x.Schedule.EndTime
            })
            .FirstOrDefaultAsync(token);

        return conflict;
    }

    public async Task<(IEnumerable<EquipmentMaintainSchedule>, int)> GetByManagerIdAsync(
        Guid userId,
        DateTime? fromDate,
        DateTime? toDate,
        MaintenanceStatus? status,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken token)
    {
        // 1. Khởi tạo Query & Include
        var query = dbContext.EquipmentMaintainSchedules
            .Include(s => s.Details)
                .ThenInclude(d => d.Equipment)
                    .ThenInclude(e => e.LabRoom) // Include để check Manager
            .AsNoTracking()
            .AsQueryable();

        // 2. Lọc theo Manager (Quyền hạn)
        // Chỉ lấy lịch bảo trì có thiết bị thuộc phòng do User này quản lý
        // (Logic này tùy thuộc vào nghiệp vụ của bạn, đây là ví dụ chuẩn)
        query = query.Where(s => s.Details.Any(d =>
            d.Equipment.LabRoom != null && d.Equipment.LabRoom.MainManagerId == userId));

        // 3. Lọc theo Date
        if (fromDate.HasValue)
            query = query.Where(s => s.StartTime >= fromDate.Value.ToUniversalTime());

        if (toDate.HasValue)
            query = query.Where(s => s.StartTime <= toDate.Value.ToUniversalTime());

        // 4. Lọc theo Status
        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        // 5. Đếm tổng số (Total Count) - Quan trọng cho phân trang
        var totalCount = await query.CountAsync(token);

        // 6. Sắp xếp (Sorting)
        if (string.IsNullOrEmpty(sortBy)) sortBy = "date";

        switch (sortBy.ToLower())
        {
            case "status":
                query = isDescending
                    ? query.OrderByDescending(s => s.Status)
                    : query.OrderBy(s => s.Status);
                break;
            case "date":
            default:
                query = isDescending
                    ? query.OrderByDescending(s => s.StartTime)
                    : query.OrderBy(s => s.StartTime);
                break;
        }

        // 7. Phân trang (Pagination) - THÊM MỚI
        var schedules = await query
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(token);

        return (schedules, totalCount);
    }

    public async Task<(IEnumerable<EquipmentMaintainSchedule>, int)> GetAllAsync(
        DateTime? fromDate,
        DateTime? toDate,
        MaintenanceStatus? status,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken token)
    {
        var query = dbContext.EquipmentMaintainSchedules
            .Include(s => s.Details)
                .ThenInclude(d => d.Equipment)
                    .ThenInclude(e => e.LabRoom)
            .AsNoTracking()
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(s => s.StartTime >= fromDate.Value.ToUniversalTime());

        if (toDate.HasValue)
            query = query.Where(s => s.StartTime <= toDate.Value.ToUniversalTime());

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        var totalCount = await query.CountAsync(token);

        if (string.IsNullOrEmpty(sortBy)) sortBy = "date";

        switch (sortBy.ToLower())
        {
            case "status":
                query = isDescending
                    ? query.OrderByDescending(s => s.Status)
                    : query.OrderBy(s => s.Status);
                break;
            case "date":
            default:
                query = isDescending
                    ? query.OrderByDescending(s => s.StartTime)
                    : query.OrderBy(s => s.StartTime);
                break;
        }

        var schedules = await query
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(token);

        return (schedules, totalCount);
    }

    public async Task<EquipmentMaintainSchedule?> GetByIdWithDetailsAsync(Guid id, CancellationToken token)
    {
        // Load sâu 3 cấp: Lịch -> Chi tiết -> Thiết bị -> Phòng Lab
        // Để phục vụ việc check quyền Manager và đổi trạng thái thiết bị
        return await dbContext.EquipmentMaintainSchedules
            .Include(s => s.Details)
                .ThenInclude(d => d.Equipment)
                    .ThenInclude(e => e.LabRoom)
            .FirstOrDefaultAsync(s => s.Id == id, token);
    }

    public async Task DeleteAsync(EquipmentMaintainSchedule schedule, CancellationToken token)
    {
        dbContext.EquipmentMaintainSchedules.Remove(schedule);
        await dbContext.SaveChangesAsync(token);
    }
}

