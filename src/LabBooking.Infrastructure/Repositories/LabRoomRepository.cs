using LabBooking.Application.Features.LabRooms.Dtos;
using LabBooking.Domain.NonEntities;

namespace LabBooking.Infrastructure.Repositories;

internal class LabRoomRepository(LabBookingDbContext dbContext) : ILabRoomRepository
{
    public async Task<Guid> Create(LabRoom entity, CancellationToken cancellationToken = default)
    {
        dbContext.LabRooms.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<LabRoom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var labRoom = await dbContext.LabRooms
            .Include(x => x.Equipments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return labRoom;
    }

    public async Task Update(LabRoom entity, CancellationToken cancellationToken = default)
    {
        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(LabRoom entity, CancellationToken cancellationToken = default)
    {
        dbContext.LabRooms.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.LabRooms.AnyAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<(IEnumerable<LabRoom>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection, CancellationToken cancellationToken = default)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = dbContext
            .LabRooms
            .Include(x => x.Equipments)
            .Include(r => r.MainManager)
            .Where(r => searchPhraseLower == null ||
                        (r.LabName != null && r.LabName.ToLower().Contains(searchPhraseLower)) ||
                        (r.Location != null && r.Location.ToLower().Contains(searchPhraseLower)));

        var totalCount = await baseQuery.CountAsync();

        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<LabRoom, object>>>
            {
                { nameof(LabRoom.LabName), r => r.LabName! },
                { nameof(LabRoom.Location), r => r.Location! },
                { nameof(LabRoom.CreatedDate), r => r.CreatedDate }
            };

            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var labRooms = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (labRooms, totalCount);
    }

    public async Task<IEnumerable<LabRoom>> GetUnmaintainedLabRoomsAsync(CancellationToken cancellationToken = default)
    {
        // Lấy danh sách LabRoomIds có lịch bảo trì với trạng thái là NotYet
        var labRoomIdsWithPendingMaintenance = dbContext.RoomMaintainSchedules
            .Where(s => s.RoomMaintainStatus == RoomMaintainStatus.NotYet)
            .Select(s => s.LabRoomId)
            .Distinct(); // Đảm bảo chỉ lấy ID duy nhất (vì một phòng có thể có nhiều lịch)

        // Lấy các LabRoom tương ứng
        var unmaintainedLabRooms = await dbContext.LabRooms
            .Where(r => !labRoomIdsWithPendingMaintenance.Contains(r.Id))
            .Include(lab => lab.Equipments)
            .ToListAsync(cancellationToken);

        return unmaintainedLabRooms;
    }

    public async Task<bool> IsLabNameUniqueAsync(string labName, CancellationToken cancellationToken = default)
    {
        var labNameLower = labName.ToLower();
        return !await dbContext.LabRooms
            .AnyAsync(r => r.LabName != null && r.LabName.ToLower() == labNameLower, cancellationToken);
    }

    public async Task<bool> IsLabNameUniqueAsync(Guid id, string labName, CancellationToken cancellationToken = default)
    {
        var isDuplicate = await dbContext.LabRooms
            .AnyAsync(room => room.LabName == labName && room.Id != id, cancellationToken);

        return !isDuplicate;
    }

    public async Task<IEnumerable<LabRoom>> GetLabsByManagerWithEquipmentsAsync(Guid managerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.LabRooms
            .Where(r => r.MainManagerId == managerId && r.IsActive)
            .Include(r => r.Equipments)
                .ThenInclude(e => e.EquipmentCategory)
            .OrderBy(r => r.LabName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MonthlyTopLabDto>> GetTopLabPerMonthAsync(int year, CancellationToken token)
    {
        // Bước 1: Query Database để lấy thống kê thô
        // Gom nhóm theo (Tháng, Phòng) và đếm số lượng
        var rawStats = await dbContext.Bookings
            .AsNoTracking()
            .Where(b => b.Status == BookingStatus.Approved) // Chỉ tính đơn đã duyệt
            .Where(b => b.CreatedAt.HasValue && b.CreatedAt.Value.Year == year) // Lọc theo năm user chọn
            .GroupBy(b => new
            {
                Month = b.CreatedAt.Value.Month,
                LabId = b.LabRoomId,
                LabName = b.LabRoom.LabName
            })
            .Select(g => new
            {
                Month = g.Key.Month,
                LabName = g.Key.LabName ?? "Unknown",
                Count = g.Count()
            })
            .ToListAsync(token);

        // Bước 2: Xử lý Logic tìm Top 1 mỗi tháng (Làm trên RAM cho dễ)
        var result = rawStats
            .GroupBy(x => x.Month) // Gom lại theo tháng
            .Select(g =>
            {
                // Trong mỗi tháng, tìm phòng có Count cao nhất
                var topRoom = g.OrderByDescending(x => x.Count).First();

                return new MonthlyTopLabDto
                {
                    MonthYear = $"{topRoom.Month}/{year}",
                    LabName = topRoom.LabName,
                    TotalBookings = topRoom.Count
                };
            })
            .OrderBy(x => x.MonthYear) // Sắp xếp thời gian
            .ToList();

        return result;
    }

    public async Task<IEnumerable<LabStatModel>> GetRawStatisticsAsync(int year, CancellationToken cancellationToken)
    {
        // Query vào BookingSlot vì nó chứa ngày tháng
        return await dbContext.Set<BookingSlot>()
            .AsNoTracking() // Read-only nên dùng AsNoTracking cho nhẹ
            .Include(bs => bs.Booking)
                .ThenInclude(b => b.LabRoom)
            // Lọc dữ liệu: Cùng năm, Slot Active, Có Booking và Phòng hợp lệ
            .Where(bs => bs.Date.Year == year
                         && bs.Status == BookingSlotStatus.Active
                         && bs.Booking != null
                         && bs.Booking.LabRoom != null)
            // Map trực tiếp sang Domain Model
            .Select(bs => new LabStatModel
            {
                LabId = bs.Booking!.LabRoomId,
                LabName = bs.Booking.LabRoom!.LabName ?? "Unknown",
                Month = bs.Date.Month,
                BookingId = bs.BookingId,
                // Logic xác định bảo trì (tùy chỉnh theo enum của bạn)
                IsMaintenance = bs.Reason == UnavailableReason.Maintenance
                                || bs.Priority == 0 // 0 is Maintenance
            })
            .ToListAsync(cancellationToken);
    }

    // Trong file LabRoomRepository.cs

    public async Task<IEnumerable<LabAvailabilityModel>> GetAvailableLabsByDateAsync(DateOnly date, CancellationToken token = default)
    {
        // 1. Lấy danh sách tất cả các Slot (Ca học) chuẩn
        var allSlots = await dbContext.Slots
            .AsNoTracking()
            .OrderBy(s => s.SlotIndex)
            .ToListAsync(token);

        // 2. Lấy danh sách tất cả Phòng Lab đang hoạt động
        var allLabs = await dbContext.LabRooms
            .AsNoTracking()
            .Where(r => r.IsActive)
            .ToListAsync(token);

        // 3. Tìm các Booking Slot đã ĐƯỢC DUYỆT (Active) vào ngày này
        // (Những slot này làm phòng bị bận)
        var bookedSlots = await dbContext.BookingSlots
            .AsNoTracking()
            .Include(bs => bs.Booking) // Include để check LabId
            .Where(bs => bs.Date == date
                         && bs.Status == BookingSlotStatus.Active // Chỉ lấy slot active
                         && bs.Booking.Status == BookingStatus.Approved) // Chỉ lấy đơn đã duyệt
            .Select(bs => new
            {
                LabId = bs.Booking.LabRoomId,
                SlotId = bs.SlotId
            })
            .ToListAsync(token);

        // 4. Tìm các Slot bị chiếm bởi LỊCH BẢO TRÌ (Maintenance)
        // Cần convert ngày chọn sang khoảng thời gian UTC để so sánh với StartTime/EndTime của bảng Maintenance

        // Giả sử múi giờ VN là UTC+7 (Cần nhất quán với logic lưu trữ của bạn)
        var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        // Ngày bắt đầu (00:00 VN) -> UTC
        var startOfDayLocal = date.ToDateTime(TimeOnly.MinValue);
        var startOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(startOfDayLocal, vnTimeZone);

        // Ngày kết thúc (23:59 VN) -> UTC
        var endOfDayLocal = date.ToDateTime(TimeOnly.MaxValue);
        var endOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(endOfDayLocal, vnTimeZone);

        var maintenanceSchedules = await dbContext.RoomMaintainSchedules
            .AsNoTracking()
            .Where(m => m.RoomMaintainStatus == RoomMaintainStatus.NotYet // Chưa xong thì tính là bận
                                                                          // Logic trùng lặp thời gian: (StartA < EndB) && (EndA > StartB)
                        && m.StartTime < endOfDayUtc
                        && m.EndTime > startOfDayUtc)
            .Select(m => new { m.LabRoomId, m.StartTime, m.EndTime })
            .ToListAsync(token);

        // 5. Xử lý Logic tính toán (In-Memory)
        var result = new List<LabAvailabilityModel>();

        foreach (var lab in allLabs)
        {
            // Danh sách các slot bị bận của phòng này
            var busySlotIds = new HashSet<Guid>();

            // a. Check bận do Booking
            var bookingConflicts = bookedSlots
                .Where(b => b.LabId == lab.Id)
                .Select(b => b.SlotId);

            foreach (var id in bookingConflicts) busySlotIds.Add(id);

            // b. Check bận do Bảo Trì
            // (Phải so sánh giờ của Slot với khoảng thời gian bảo trì)
            var labMaintenances = maintenanceSchedules.Where(m => m.LabRoomId == lab.Id).ToList();

            if (labMaintenances.Any())
            {
                foreach (var slot in allSlots)
                {
                    // Convert giờ Slot sang UTC (dựa trên ngày đang check)
                    var slotStartLocal = date.ToDateTime(slot.StartTime);
                    var slotEndLocal = date.ToDateTime(slot.EndTime);

                    var slotStartUtc = TimeZoneInfo.ConvertTimeToUtc(slotStartLocal, vnTimeZone);
                    var slotEndUtc = TimeZoneInfo.ConvertTimeToUtc(slotEndLocal, vnTimeZone);

                    // Nếu slot này nằm trong khoảng bảo trì -> Bận
                    bool isMaintain = labMaintenances.Any(m =>
                        m.StartTime < slotEndUtc && m.EndTime > slotStartUtc);

                    if (isMaintain)
                    {
                        busySlotIds.Add(slot.Id);
                    }
                }
            }

            // c. Tìm các Slot còn trống (Total - Busy)
            var availableSlots = allSlots
              .Where(s => !busySlotIds.Contains(s.Id))
              .Select(s => new SlotTimeModel
              {
                  Id = s.Id,
                  SlotIndex = s.SlotIndex,
                  StartTime = s.StartTime, // Giữ nguyên TimeOnly
                  EndTime = s.EndTime      // Giữ nguyên TimeOnly
              })
              .ToList();

            // d. Thêm vào kết quả
            result.Add(new LabAvailabilityModel
            {
                LabId = lab.Id,
                LabName = lab.LabName ?? "",
                Location = lab.Location ?? "",
                Capacity = lab.MaximumLimit ?? 0,
                AvailableSlots = availableSlots
            });
        }

        return result;
    }
}
