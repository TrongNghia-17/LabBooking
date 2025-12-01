using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;
using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetAll;

public class GetAllMaintainSchedulesQueryHandler(
    IEquipmentMaintainScheduleRepository repository,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IRequestHandler<GetAllMaintainSchedulesQuery, IEnumerable<EquipmentMaintainScheduleResponse>>
{
    public async Task<IEnumerable<EquipmentMaintainScheduleResponse>> Handle(GetAllMaintainSchedulesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");

        var schedules = await repository.GetByManagerIdAsync(
            userId,
            request.FromDate,
            request.ToDate,
            request.Status,
            request.SortBy,
            request.IsDescending,
            cancellationToken);

        return mapper.Map<IEnumerable<EquipmentMaintainScheduleResponse>>(schedules);
    }
}
