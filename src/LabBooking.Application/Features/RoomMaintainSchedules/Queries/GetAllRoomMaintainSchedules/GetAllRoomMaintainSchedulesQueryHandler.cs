using LabBooking.Application.Features.RoomMaintainSchedules.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Queries.GetAllRoomMaintainSchedules;

public class GetAllRoomMaintainSchedulesQueryHandler(
    ILogger<GetAllRoomMaintainSchedulesQueryHandler> logger,
    IRoomMaintainScheduleRepository repository,
    IMapper mapper,
    ICurrentUserService currentUserService,
    UserManager<User> userManager // Cần UserManager để check Role
) : IRequestHandler<GetAllRoomMaintainSchedulesQuery, PagedResult<RoomMaintainScheduleResponse>>
{
    public async Task<PagedResult<RoomMaintainScheduleResponse>> Handle(GetAllRoomMaintainSchedulesQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy User ID hiện tại
        var currentUserId = currentUserService.UserId;
        if (currentUserId == null)
        {
            throw new UnauthorizedAccessException("Bạn cần đăng nhập.");
        }

        // 2. Xác định quyền hạn (Admin hay Manager)
        Guid? managerIdToFilter = null; // Mặc định là null (xem tất cả - dành cho Admin)

        var user = await userManager.FindByIdAsync(currentUserId.Value.ToString());
        if (user != null)
        {
            var roles = await userManager.GetRolesAsync(user);
            bool isAdmin = roles.Contains("Admin");
            bool isManager = roles.Contains("Manager");

            // LOGIC QUYẾT ĐỊNH:
            // - Nếu là Admin: managerIdToFilter = null (để Repo không lọc -> lấy hết)
            // - Nếu KHÔNG phải Admin và LÀ Manager: managerIdToFilter = currentUserId (để Repo lọc theo ID này)
            if (!isAdmin && isManager)
            {
                managerIdToFilter = currentUserId;
                logger.LogInformation("User {UserId} là Manager, chỉ lấy lịch bảo trì thuộc quyền quản lý.", currentUserId);
            }
            else
            {
                logger.LogInformation("User {UserId} là Admin (hoặc quyền cao nhất), lấy toàn bộ lịch bảo trì.", currentUserId);
            }
        }

        // 3. Gọi Repository
        var (schedules, totalCount) = await repository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.Status,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            managerIdToFilter, // Truyền ID lọc (hoặc null) vào đây
            cancellationToken);

        // 4. Map và trả về
        var dtos = mapper.Map<IEnumerable<RoomMaintainScheduleResponse>>(schedules);

        return new PagedResult<RoomMaintainScheduleResponse>(
            dtos,
            totalCount,
            request.PageSize,
            request.PageNumber);
    }
}
