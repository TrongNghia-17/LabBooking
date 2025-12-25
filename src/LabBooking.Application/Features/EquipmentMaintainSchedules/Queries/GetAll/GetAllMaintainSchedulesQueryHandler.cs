using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetAll;

public class GetAllMaintainSchedulesQueryHandler(
    IEquipmentMaintainScheduleRepository repository,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IRequestHandler<GetAllMaintainSchedulesQuery, PagedResult<EquipmentMaintainScheduleResponse>>
{
    public async Task<PagedResult<EquipmentMaintainScheduleResponse>> Handle(GetAllMaintainSchedulesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");

        IEnumerable<EquipmentMaintainSchedule> schedules;
        int totalCount;

        if (currentUserService.Roles.Contains(Roles.Admin))
        {
            // Nếu là Admin, gọi phương thức GetAllAsync để lấy tất cả lịch
            (schedules, totalCount) = await repository.GetAllAsync(
                request.FromDate,
                request.ToDate,
                request.Status,
                request.SortBy,
                request.IsDescending,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }
        else
        {
            // Nếu không phải Admin (mặc định là Manager), giữ nguyên logic cũ
            (schedules, totalCount) = await repository.GetByManagerIdAsync(
                userId,
                request.FromDate,
                request.ToDate,
                request.Status,
                request.SortBy,
                request.IsDescending,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }

        // Map sang DTO
        var dtos = mapper.Map<IEnumerable<EquipmentMaintainScheduleResponse>>(schedules);

        // Trả về PagedResult
        return new PagedResult<EquipmentMaintainScheduleResponse>(
            dtos,
            totalCount,
            request.PageSize,
            request.PageNumber);
    }
}
