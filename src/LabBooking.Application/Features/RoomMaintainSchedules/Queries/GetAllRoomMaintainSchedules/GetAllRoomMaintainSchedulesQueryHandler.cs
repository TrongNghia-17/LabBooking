using LabBooking.Application.Features.RoomMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Queries.GetAllRoomMaintainSchedules;

public class GetAllRoomMaintainSchedulesQueryHandler(
    ILogger<GetAllRoomMaintainSchedulesQueryHandler> logger,
    IRoomMaintainScheduleRepository roomMaintainScheduleRepository,
    IMapper mapper
) : IRequestHandler<GetAllRoomMaintainSchedulesQuery, PagedResult<RoomMaintainScheduleResponse>>
{
    public async Task<PagedResult<RoomMaintainScheduleResponse>> Handle(GetAllRoomMaintainSchedulesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang lấy danh sách room maintain schedules...");

        // 1. Gọi Repository (đã bỏ LabRoomId)
        var (schedules, totalCount) = await roomMaintainScheduleRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.Status, // Tham số lọc trạng thái
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        // 2. Map sang DTO
        var schedulesResponse = mapper.Map<IEnumerable<RoomMaintainScheduleResponse>>(schedules);

        // 3. Đóng gói kết quả PagedResult
        var result = new PagedResult<RoomMaintainScheduleResponse>(
            schedulesResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);

        return result;
    }
}
