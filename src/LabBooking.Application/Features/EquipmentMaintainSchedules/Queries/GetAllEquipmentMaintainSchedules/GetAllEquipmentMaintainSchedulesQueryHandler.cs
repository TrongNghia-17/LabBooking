using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetAllEquipmentMaintainSchedules;

public class GetAllEquipmentMaintainSchedulesQueryHandler(
    ILogger<GetAllEquipmentMaintainSchedulesQueryHandler> logger,
    IEquipmentMaintainScheduleRepository equipmentMaintainScheduleRepository,
    IMapper mapper
) : IRequestHandler<GetAllEquipmentMaintainSchedulesQuery, PagedResult<EquipmentMaintainScheduleResponse>>
{
    public async Task<PagedResult<EquipmentMaintainScheduleResponse>> Handle(GetAllEquipmentMaintainSchedulesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang lấy danh sách equipment maintain schedules...");

        // 1. Gọi Repository (đã bỏ EquipmentId)
        var (schedules, totalCount) = await equipmentMaintainScheduleRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.Status, // Tham số lọc trạng thái
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        // 2. Map sang DTO
        var schedulesResponse = mapper.Map<IEnumerable<EquipmentMaintainScheduleResponse>>(schedules);

        // 3. Đóng gói kết quả PagedResult
        var result = new PagedResult<EquipmentMaintainScheduleResponse>(
            schedulesResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);

        return result;
    }
}