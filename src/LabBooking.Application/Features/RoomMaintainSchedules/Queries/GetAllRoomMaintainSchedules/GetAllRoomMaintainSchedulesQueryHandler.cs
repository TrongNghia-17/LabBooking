using LabBooking.Application.Features.RoomMaintainSchedules.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Queries.GetAllRoomMaintainSchedules;

public class GetAllRoomMaintainSchedulesQueryHandler(
    ILogger<GetAllRoomMaintainSchedulesQueryHandler> logger,
    IRoomMaintainScheduleRepository roomMaintainScheduleRepository,
    IMapper mapper,
    ICurrentUserService currentUserService,
    UserManager<User> userManager
) : IRequestHandler<GetAllRoomMaintainSchedulesQuery, PagedResult<RoomMaintainScheduleResponse>>
{
    public async Task<PagedResult<RoomMaintainScheduleResponse>> Handle(GetAllRoomMaintainSchedulesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang lấy danh sách room maintain schedules...");

        var currentUserId = currentUserService.UserId;
        Guid? managerIdToFilter = null;

        if (currentUserId != null)
        {
            var user = await userManager.FindByIdAsync(currentUserId.Value.ToString());
            if (user != null)
            {
                var isManager = await userManager.IsInRoleAsync(user, "Manager");
                var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

                if (isManager && !isAdmin)
                {
                    managerIdToFilter = currentUserId;
                }
            }
        }

        var (schedules, totalCount) = await roomMaintainScheduleRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.Status,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            managerIdToFilter,
            cancellationToken);

        var schedulesResponse = mapper.Map<IEnumerable<RoomMaintainScheduleResponse>>(schedules);

        var result = new PagedResult<RoomMaintainScheduleResponse>(
            schedulesResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);

        return result;
    }
}
