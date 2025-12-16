using LabBooking.Domain.Enums;

namespace LabBooking.Infrastructure.Repositories;

internal class DoorRequestRepository(LabBookingDbContext dbContext, INotificationRepository notificationRepo) : IDoorRequestRepository
{
    // 1. Tạo yêu cầu mới
    public async Task CreateAsync(DoorOpeningRequest request, CancellationToken token)
    {
        var pushQueue = new List<PushNotificationData>();

        // 1. Lấy tên phòng Lab để nội dung thông báo rõ ràng hơn
        var labName = await dbContext.LabRooms
            .Where(l => l.Id == request.LabRoomId)
            .Select(l => l.LabName)
            .FirstOrDefaultAsync(token) ?? "Phòng Lab";

        await dbContext.DoorOpeningRequests.AddAsync(request, token);

        var guardRoleName = "SecurityGuard";

        var guardIds = await (from user in dbContext.Users
                              join userRole in dbContext.UserRoles on user.Id equals userRole.UserId
                              join role in dbContext.Roles on userRole.RoleId equals role.Id
                              where role.Name == guardRoleName
                              select user.Id)
                                 .ToListAsync(token);

        if (guardIds.Any())
        {
            var title = "🔑 Yêu cầu mở cửa mới";
            var message = $"Có yêu cầu mở cửa tại {labName}. Vui lòng kiểm tra.";

            foreach (var guardId in guardIds)
            {
                var (_, pushData) = notificationRepo.PrepareNotification(
                    guardId,
                    title,
                    message,
                    "DOOR_OPENING_REQUEST",
                    new { requestId = request.Id, labId = request.LabRoomId }
                );

                pushQueue.Add(pushData);
            }
        }
        await dbContext.SaveChangesAsync(token);

        notificationRepo.RunPushNotificationTask(pushQueue);
    }

    // 2. Check Spam: User này có đang treo yêu cầu nào ở phòng này không?
    public async Task<bool> HasPendingRequestAsync(Guid userId, Guid labRoomId, CancellationToken token)
    {
        return await dbContext.DoorOpeningRequests
            .AnyAsync(r => r.RequestedById == userId
                        && r.LabRoomId == labRoomId
                        && r.Status == DoorRequestStatus.Pending, token);
    }

    // 3. Lấy danh sách cho Bảo vệ xem (Kèm thông tin Sinh viên)
    public async Task<IEnumerable<DoorOpeningRequest>> GetPendingRequestsForGuardAsync(CancellationToken token)
    {
        return await dbContext.DoorOpeningRequests
            .Include(r => r.LabRoom)      // Lấy tên phòng
            .Include(r => r.RequestedBy)  // Lấy tên & MSSV người yêu cầu
            .Where(r => r.Status == DoorRequestStatus.Pending)
            .OrderBy(r => r.RequestTime)  // Ai gọi trước hiện trước
            .ToListAsync(token);
    }

    public async Task<DoorOpeningRequest?> GetByIdAsync(Guid id, CancellationToken token)
    {
        return await dbContext.DoorOpeningRequests
            .FirstOrDefaultAsync(x => x.Id == id, token);
    }

    // 2. Cập nhật (Update) xuống DB
    public async Task UpdateAsync(DoorOpeningRequest request, CancellationToken token)
    {
        dbContext.DoorOpeningRequests.Update(request);
        await dbContext.SaveChangesAsync(token);
    }
    public async Task<IEnumerable<DoorOpeningRequest>> GetHistoryAsync(
    Guid currentUserId,
    bool canViewAll,
    Guid? roomId,
    DoorRequestStatus? status,
    DateTime? from,
    DateTime? to,
    CancellationToken token)
    {
        var query = dbContext.DoorOpeningRequests
            .AsNoTracking()
            .Include(x => x.LabRoom)      // Lấy tên phòng
            .Include(x => x.RequestedBy)  // Lấy tên sinh viên
            .Include(x => x.HandledBy)    // Lấy tên bảo vệ
            .AsQueryable();

        // 1. PHÂN QUYỀN DỮ LIỆU
        if (!canViewAll)
        {
            // Nếu không phải Bảo vệ/Admin -> Chỉ lấy cái do chính mình tạo
            query = query.Where(x => x.RequestedById == currentUserId);
        }
        // Nếu là Bảo vệ (canViewAll = true) -> Không filter dòng trên -> Xem hết

        // 2. CÁC BỘ LỌC BỔ SUNG
        if (roomId.HasValue)
            query = query.Where(x => x.LabRoomId == roomId.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (from.HasValue)
            query = query.Where(x => x.RequestTime >= from.Value.ToUniversalTime()); // Nhớ UTC

        if (to.HasValue)
            query = query.Where(x => x.RequestTime <= to.Value.ToUniversalTime());

        // 3. SẮP XẾP (Mới nhất lên đầu)
        query = query.OrderByDescending(x => x.RequestTime);

        return await query.ToListAsync(token);
    }

    // Thêm method này vào file DoorRequestRepository.cs
    public async Task DeleteAsync(DoorOpeningRequest request, CancellationToken token)
    {
        // Lệnh này sẽ xóa hẳn dòng dữ liệu khỏi bảng (Hard Delete)
        dbContext.DoorOpeningRequests.Remove(request);
        await dbContext.SaveChangesAsync(token);
    }

    // Trong class BookingRepository

    public async Task<List<BookingSlot>> GetBookingsEligibleForDoorOpenAsync(Guid userId)
    {
        // 1. Xác định thời gian hiện tại ở Việt Nam
        var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var nowUtc = DateTime.UtcNow;
        var nowVn = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, vnTimeZone);

        var today = DateOnly.FromDateTime(nowVn);
        var currentTime = TimeOnly.FromDateTime(nowVn);

        // 2. Lấy danh sách các slot đã book của user trong HÔM NAY
        // Chỉ lấy Active slot và Approved booking
        var userSlotsToday = await dbContext.BookingSlots
            .AsNoTracking()
            .Include(bs => bs.Booking)
                .ThenInclude(b => b.LabRoom)
            .Include(bs => bs.Slot)
            .Where(bs =>
                bs.Date == today &&
                bs.Status == BookingSlotStatus.Active &&
                bs.Booking.CreatedById == userId &&
                bs.Booking.Status == BookingStatus.Approved
            )
            .ToListAsync();

        // 3. Lọc theo Business Rule: Chỉ hiển thị trước 1 tiếng so với giờ bắt đầu
        // VÀ vẫn hiển thị trong lúc đang diễn ra slot (để lỡ user ra ngoài cần vào lại)

        var eligibleSlots = userSlotsToday.Where(bs =>
        {
            // Ví dụ: Slot bắt đầu 07:00.
            // allowTime = 06:00
            var allowTime = bs.Slot.StartTime.AddHours(-1);

            // Điều kiện: Thời gian hiện tại >= 06:00 VÀ Thời gian hiện tại <= Giờ kết thúc slot
            return currentTime >= allowTime && currentTime <= bs.Slot.EndTime;
        }).ToList();

        return eligibleSlots;
    }
}
