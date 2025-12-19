using LabBooking.Domain.Enums;
using LabBooking.Domain.Exceptions;

namespace LabBooking.Infrastructure.Repositories;

internal class IncidentRepository(LabBookingDbContext dbContext, INotificationRepository notificationRepo) : IIncidentRepository
{
    public async Task<Guid> CreateAsync(Incident incident, CancellationToken token)
    {
        var pushQueue = new List<PushNotificationData>();

        // 1. Lấy thông tin Manager của phòng Lab liên quan
        var labInfo = await dbContext.LabRooms
            .Where(l => l.Id == incident.LabRoomId)
            .Select(l => new { l.MainManagerId, l.LabName })
            .FirstOrDefaultAsync(token);

        if (labInfo == null)
            throw new NotFoundException("LabRoom", incident.LabRoomId.ToString());

        await dbContext.Incidents.AddAsync(incident, token);

        var notiMessage = !string.IsNullOrEmpty(incident.Description)
                ? $"Sự cố mới tại {labInfo.LabName}: {incident.Description}"
                : $"Có báo cáo sự cố mới tại {labInfo.LabName} cần bạn kiểm tra.";

        // Cắt ngắn message nếu quá dài để hiển thị thông báo đẹp hơn
        if (notiMessage.Length > 100) notiMessage = notiMessage.Substring(0, 97) + "...";

        var (_, mgrPush) = notificationRepo.PrepareNotification(
            labInfo.MainManagerId,
            "🚨 Báo cáo sự cố mới",
            notiMessage,
            "MANAGER_NEW_INCIDENT",
            new { incidentId = incident.Id, labRoomId = incident.LabRoomId }
        );
        pushQueue.Add(mgrPush);

        await dbContext.SaveChangesAsync(token);

        notificationRepo.RunPushNotificationTask(pushQueue);

        return incident.Id;
    }

    public async Task<(IEnumerable<Incident>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = dbContext
            .Incidents
            .Where(r => searchPhraseLower == null || r.Description.ToLower().Contains(searchPhraseLower));

        var totalCount = await baseQuery.CountAsync();

        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<Incident, object>>>
            {
                { nameof(Incident.Description), r => r.Description },
            };

            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var labs = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (labs, totalCount);
    }

    public async Task<bool> IsSpamAsync(Guid userId, Guid labRoomId, IncidentType type, CancellationToken token)
    {
        var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);

        return await dbContext.Incidents
            .AnyAsync(x => x.ReportedById == userId
                        && x.LabRoomId == labRoomId
                        && x.Type == type
                        && x.CreatedAt > oneMinuteAgo, token);
    }

    public async Task DeleteAsync(Incident incident, CancellationToken token)
    {
        dbContext.Incidents.Remove(incident);
        await dbContext.SaveChangesAsync(token);
    }

    // 1. Hàm lấy cho Manager (Lấy tất cả sự cố trong các phòng Manager này quản lý)
    public async Task<IEnumerable<Incident>> GetByManagerIdAsync(Guid managerId, CancellationToken token)
    {
        return await dbContext.Incidents
            .Include(i => i.LabRoom)
            .Include(i => i.IncidentEquipments)
        .ThenInclude(ie => ie.Equipment)
            .Include(i => i.ReportedBy) // Manager cần thông tin người báo
            .Where(i => i.LabRoom.MainManagerId == managerId) // <--- Logic lọc theo quyền quản lý
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(token);
    }

    // 2. Hàm lấy cho Cá nhân (Đã có từ trước)
    public async Task<IEnumerable<Incident>> GetByReporterIdAsync(Guid reporterId, CancellationToken token)
    {
        return await dbContext.Incidents
            .Include(i => i.LabRoom)
            .Include(i => i.IncidentEquipments)
        .ThenInclude(ie => ie.Equipment)
            // Không cần Include ReportedBy cũng được vì Guard không cần xem
            .Where(i => i.ReportedById == reporterId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(token);
    }

    public async Task<IEnumerable<Incident>> GetFilteredAsync(
    Guid? managerId,   // Nếu != null -> Chỉ lấy phòng do ông này quản lý
    Guid? reporterId,  // Nếu != null -> Chỉ lấy incident do ông này tạo
    Guid? labRoomId,   // Lọc theo phòng cụ thể
    DateTime? from,
    DateTime? to,
    bool? isResolved,
    LevelOfImportance? importance,
    bool isDescending,
    CancellationToken token)
    {
        var query = dbContext.Incidents
            .Include(i => i.LabRoom)
            .Include(i => i.IncidentEquipments)
        .ThenInclude(ie => ie.Equipment)
            .Include(i => i.ReportedBy)
            .AsQueryable();

        // 1. LOGIC MANAGER (Bị giới hạn quyền)
        if (managerId.HasValue)
        {
            // Bắt buộc: Incident phải thuộc phòng do Manager này quản lý
            query = query.Where(i => i.LabRoom.MainManagerId == managerId.Value);
        }

        // 2. LOGIC REPORTER (Nếu muốn xem của riêng mình - Dành cho SV/GV)
        if (reporterId.HasValue)
        {
            query = query.Where(i => i.ReportedById == reporterId.Value);
        }

        // 3. LOGIC LỌC PHÒNG (Guard chọn phòng để xem)
        if (labRoomId.HasValue)
        {
            query = query.Where(i => i.LabRoomId == labRoomId.Value);
        }

        // 4. CÁC BỘ LỌC KHÁC (Chung cho tất cả)
        if (from.HasValue)
            query = query.Where(i => i.CreatedAt >= from.Value.ToUniversalTime());

        if (to.HasValue)
            query = query.Where(i => i.CreatedAt <= to.Value.ToUniversalTime());

        if (isResolved.HasValue)
            query = query.Where(i => i.IsResolved == isResolved.Value);

        if (importance.HasValue)
            query = query.Where(i => i.ImportanceLevel == importance.Value);

        // 5. SẮP XẾP
        query = isDescending
            ? query.OrderByDescending(i => i.CreatedAt)
            : query.OrderBy(i => i.CreatedAt);

        return await query.ToListAsync(token);
    }

    public async Task<Incident?> GetByIdWithDetailsAsync(Guid id, CancellationToken token)
    {
        return await dbContext.Incidents
            .Include(i => i.ReportedBy)
            .Include(i => i.LabRoom)
            .Include(i => i.IncidentEquipments)
                .ThenInclude(ie => ie.Equipment) // Include sâu để lấy trạng thái thiết bị
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted, token); // Chỉ lấy cái chưa xóa
    }

    public async Task SoftDeleteWithRestoreDevicesAsync(Incident incident, CancellationToken token)
    {
        using var transaction = await dbContext.Database.BeginTransactionAsync(token);
        try
        {
            // 1. Phục hồi thiết bị (Nếu là lỗi thiết bị)
            if (incident.Type == IncidentType.EquipmentFailure && incident.IncidentEquipments.Any())
            {
                foreach (var incidentEq in incident.IncidentEquipments)
                {
                    var equipment = incidentEq.Equipment;
                    // Chỉ phục hồi nếu nó đang bị đánh dấu là Broken
                    if (equipment != null && equipment.Status == EquipmentStatus.Broken)
                    {
                        equipment.Status = EquipmentStatus.Available;
                        equipment.IsAvailable = true;
                        // Đánh dấu update
                        dbContext.Equipments.Update(equipment);
                    }
                }
            }

            // 2. Soft Delete Incident
            incident.IsDeleted = true;
            incident.DeletedAt = DateTime.UtcNow;
            dbContext.Incidents.Update(incident);

            // 3. Lưu tất cả
            await dbContext.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
        }
        catch
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    public async Task<bool> HasActiveIncidentForRoomCheckAsync(Guid roomCheckId, CancellationToken token)
    {
        return await dbContext.Incidents
            .AnyAsync(i => i.RoomCheckId == roomCheckId && !i.IsDeleted, token);
    }
}
