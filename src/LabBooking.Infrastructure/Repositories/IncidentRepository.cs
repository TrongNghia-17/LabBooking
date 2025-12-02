using LabBooking.Domain.Enums;

namespace LabBooking.Infrastructure.Repositories;

internal class IncidentRepository(LabBookingDbContext dbContext) : IIncidentRepository
{
    public async Task<Guid> CreateAsync(Incident incident, CancellationToken token)
    {
        await dbContext.Incidents.AddAsync(incident, token);
        await dbContext.SaveChangesAsync(token);

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
    public async Task<Incident?> GetByIdWithDetailsAsync(Guid id, CancellationToken token)
    {
        return await dbContext.Incidents
            .Include(i => i.ReportedBy) // Lấy thông tin người báo
            .Include(i => i.LabRoom)    // Lấy thông tin phòng (để check Manager)
            .Include(i => i.Equipment)  // Lấy thiết bị (để revert status)
            .FirstOrDefaultAsync(i => i.Id == id, token);
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
            .Include(i => i.Equipment)
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
            .Include(i => i.Equipment)
            // Không cần Include ReportedBy cũng được vì Guard không cần xem
            .Where(i => i.ReportedById == reporterId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(token);
    }

    public async Task<IEnumerable<Incident>> GetFilteredAsync(
    Guid? managerId,
    Guid? labRoomId,
    DateTime? from,
    DateTime? to,
    bool? isResolved,
    LevelOfImportance? importance,
    bool isDescending,
    CancellationToken token)
    {
        // 1. Khởi tạo Query & Include bảng liên quan
        var query = dbContext.Incidents
            .Include(i => i.LabRoom)
            .Include(i => i.Equipment)
            .Include(i => i.ReportedBy) // Để hiển thị tên người báo
            .AsQueryable();

        // 2. LOGIC PHÂN QUYỀN (Manager chỉ xem phòng mình)
        if (managerId.HasValue)
        {
            query = query.Where(i => i.LabRoom.MainManagerId == managerId.Value);
        }

        // 3. LOGIC LỌC PHÒNG LAB (Guard chọn phòng cụ thể)
        if (labRoomId.HasValue)
        {
            query = query.Where(i => i.LabRoomId == labRoomId.Value);
        }

        // 4. LOGIC LỌC NGÀY THÁNG
        if (from.HasValue)
            query = query.Where(i => i.CreatedAt >= from.Value.ToUniversalTime());

        if (to.HasValue)
            query = query.Where(i => i.CreatedAt <= to.Value.ToUniversalTime());

        // 5. LOGIC LỌC TRẠNG THÁI (Done / Not Yet)
        if (isResolved.HasValue)
            query = query.Where(i => i.IsResolved == isResolved.Value);

        // 6. LOGIC LỌC MỨC ĐỘ
        if (importance.HasValue)
            query = query.Where(i => i.ImportanceLevel == importance.Value);

        // 7. SẮP XẾP
        query = isDescending
            ? query.OrderByDescending(i => i.CreatedAt)
            : query.OrderBy(i => i.CreatedAt);

        return await query.ToListAsync(token);
    }
}
