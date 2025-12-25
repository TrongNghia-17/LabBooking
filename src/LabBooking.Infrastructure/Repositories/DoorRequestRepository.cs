using LabBooking.Application.Features.DoorRequests.Dtos;
using LabBooking.Domain.Enums;
using LabBooking.Domain.NonEntities;

namespace LabBooking.Infrastructure.Repositories;

internal class DoorRequestRepository(LabBookingDbContext dbContext) : IDoorRequestRepository
{
    public async Task<Guid> AddAsync(DoorOpeningRequest entity)
    {
        await dbContext.DoorOpeningRequests.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<bool> HasPendingRequestAsync(
        string bookingCode,
        DateOnly requestDate,
        Guid slotId)
    {
        return await dbContext.DoorOpeningRequests
            .AnyAsync(x =>
                x.BookingCode == bookingCode &&
                x.RequestDate == requestDate &&
                x.SlotId == slotId &&
                x.Status == DoorRequestStatus.Pending);
    }

    public async Task<DoorOpeningRequest?> GetByIdAsync(Guid id)
    {
        return await dbContext.DoorOpeningRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<DoorOpeningRequest?> GetByIdWithUserAsync(Guid id)
    {
        return await dbContext.DoorOpeningRequests
            .AsNoTracking()
            .Include(x => x.RequestedBy)
            .Include(x => x.Slot)
            .Include(x => x.Manager)
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task DeleteAsync(DoorOpeningRequest request)
    {
        dbContext.DoorOpeningRequests.Remove(request);
        await dbContext.SaveChangesAsync();
    }

    public async Task<(IEnumerable<DoorOpeningRequest> Items, int TotalCount)> GetPagedListAsync(
        Guid? managerId,      // Nếu có giá trị -> Lọc theo Manager
        Guid? requestedById,  // Nếu có giá trị -> Lọc theo Người tạo
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        DateOnly? filterDate,
        DoorRequestStatus? filterStatus, // Trạng thái cụ thể (Pending/Approved...)
        bool? isHistory,                 // [MỚI] True: Lấy (Approved + Rejected), False: Lấy Pending
        CancellationToken cancellationToken)
    {
        // 1. Base Query
        var baseQuery = dbContext.DoorOpeningRequests
            .Include(x => x.RequestedBy)
            .Include(x => x.Manager)
            .Include(x => x.Slot)
            .AsNoTracking();

        // 2. PHÂN QUYỀN DỮ LIỆU (QUAN TRỌNG)
        if (managerId.HasValue)
        {
            // Nếu là Manager xem -> Chỉ lấy request thuộc về manager này
            baseQuery = baseQuery.Where(r => r.ManagerId == managerId.Value);
        }
        else if (requestedById.HasValue)
        {
            // Nếu là Student/Lecturer xem -> Chỉ lấy request của chính họ
            baseQuery = baseQuery.Where(r => r.RequestedById == requestedById.Value);
        }

        // 3. Xử lý logic "Lịch sử" vs "Đang xử lý" (Nâng cao)
        if (filterStatus.HasValue)
        {
            // Nếu chọn cụ thể 1 status
            baseQuery = baseQuery.Where(r => r.Status == filterStatus.Value);
        }
        else if (isHistory.HasValue)
        {
            if (isHistory.Value == true)
            {
                // Lịch sử = Đã duyệt HOẶC Đã từ chối (Khác Pending)
                baseQuery = baseQuery.Where(r => r.Status != DoorRequestStatus.Pending);
            }
            else
            {
                // Đang xử lý = Pending
                baseQuery = baseQuery.Where(r => r.Status == DoorRequestStatus.Pending);
            }
        }

        // 4. Tìm kiếm
        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            var lowerSearchPhrase = searchPhrase.ToLower();
            baseQuery = baseQuery.Where(r =>
                r.BookingCode.ToLower().Contains(lowerSearchPhrase) ||
                r.Reason.ToLower().Contains(lowerSearchPhrase));
        }

        // 5. Lọc theo ngày
        if (filterDate.HasValue)
        {
            baseQuery = baseQuery.Where(r => DateOnly.FromDateTime(r.RequestTime) == filterDate.Value);
        }

        // 6. Sắp xếp
        baseQuery = sortBy switch
        {
            nameof(DoorRequestDto.BookingCode) => sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(r => r.BookingCode)
                : baseQuery.OrderByDescending(r => r.BookingCode),
            nameof(DoorRequestDto.Status) => sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(r => r.Status)
                : baseQuery.OrderByDescending(r => r.Status),
            _ => sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(r => r.RequestTime)
                : baseQuery.OrderByDescending(r => r.RequestTime)
        };

        // 7. Phân trang
        var totalCount = await baseQuery.CountAsync(cancellationToken);
        var items = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task UpdateAsync(DoorOpeningRequest request)
    {
        dbContext.DoorOpeningRequests.Update(request);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<DailyManagerNoteDto>> GetManagerNotesByDateAsync(DateOnly date, CancellationToken cancellationToken)
    {
        // Query: Join bảng Request với Booking để lấy tên phòng Lab
        // Chỉ lấy những đơn trạng thái Accepted (Đã duyệt)

        var query = from req in dbContext.DoorOpeningRequests
                    join booking in dbContext.Bookings
                         on req.BookingCode equals booking.QrCodeString // (Hoặc booking.Title tùy DB của bạn)
                    where req.RequestDate == date
                          && req.Status == DoorRequestStatus.Accepted
                    // Chỉ lấy những đơn CÓ NOTE (hoặc lấy tất cả tùy bạn)
                    // && !string.IsNullOrEmpty(req.ManagerNote) 
                    select new
                    {
                        req.Id,
                        req.ManagerNote,
                        req.BookingCode,
                        RequesterName = req.RequestedBy.FullName,

                        // Lấy thông tin Slot
                        StartTime = req.Slot.StartTime,
                        EndTime = req.Slot.EndTime,

                        // Lấy tên phòng từ bảng Booking
                        LabName = booking.LabRoom.LabName
                    };

        var data = await query
            .Distinct()
            .OrderBy(x => x.StartTime) // Sắp xếp theo giờ tăng dần để bảo vệ dễ theo dõi
            .ToListAsync(cancellationToken);

        // Map sang DTO
        return data.Select(x => new DailyManagerNoteDto
        {
            RequestId = x.Id,
            LabName = x.LabName ?? "Phòng Lab",
            RequesterName = x.RequesterName,
            BookingCode = x.BookingCode,

            // Format khung giờ: "13:00 - 15:00"
            SlotTime = $"{x.StartTime:HH:mm} - {x.EndTime:HH:mm}",

            // Hiển thị note (nếu null thì ghi chú mặc định)
            ManagerNote = string.IsNullOrWhiteSpace(x.ManagerNote)
                          ? "Cho phép vào (Không có ghi chú thêm)"
                          : x.ManagerNote
        }).ToList();
    }
}
