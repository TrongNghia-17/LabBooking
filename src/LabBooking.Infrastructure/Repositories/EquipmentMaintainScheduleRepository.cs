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

    public async Task<IEnumerable<EquipmentMaintainSchedule>> GetByManagerIdAsync(
        Guid managerId,
        DateTime? from,
        DateTime? to,
        MaintenanceStatus? status,
        string? sortBy,
        bool isDescending,
        CancellationToken token = default)
    {
        // 1. Khởi tạo Query (Chưa chạy xuống DB)
        var query = dbContext.EquipmentMaintainSchedules
            .Include(s => s.Details)
                .ThenInclude(d => d.Equipment)
                    .ThenInclude(e => e.LabRoom)
            .AsQueryable(); // Chuyển sang IQueryable để cộng dồn điều kiện

        // 2. LỌC THEO MANAGER (Bắt buộc)
        query = query.Where(s => s.Details.Any(d =>
            d.Equipment != null &&
            d.Equipment.LabRoom != null &&
            d.Equipment.LabRoom.MainManagerId == managerId));

        // 3. LỌC THEO NGÀY (Optional)
        if (from.HasValue)
            query = query.Where(s => s.StartTime >= from.Value.ToUniversalTime());

        if (to.HasValue)
            query = query.Where(s => s.EndTime <= to.Value.ToUniversalTime());

        // 4. LỌC THEO STATUS (Optional)
        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        // 5. SẮP XẾP (Sorting)
        // Mặc định sắp theo StartTime giảm dần nếu không truyền gì cả
        if (string.IsNullOrEmpty(sortBy)) sortBy = "Date";

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

        // 6. Thực thi truy vấn
        return await query.ToListAsync(token);
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

