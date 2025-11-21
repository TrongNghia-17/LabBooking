using LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Queries.GetEquipmentMaintainScheduleById;

public class GetEquipmentMaintainScheduleByIdQueryHandler(
    ILogger<GetEquipmentMaintainScheduleByIdQueryHandler> logger,
    IEquipmentMaintainScheduleRepository equipmentMaintainScheduleRepository,
    IMapper mapper
    ) : IRequestHandler<GetEquipmentMaintainScheduleByIdQuery, EquipmentMaintainScheduleResponse>
{
    public async Task<EquipmentMaintainScheduleResponse> Handle(GetEquipmentMaintainScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang lấy EquipmentMaintainSchedule bằng Id: {ScheduleId}", request.Id);

        // 1. Lấy entity từ repository
        var schedule = await equipmentMaintainScheduleRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(EquipmentMaintainSchedule), request.Id.ToString());

        // 2. Map entity sang DTO
        var scheduleResponse = mapper.Map<EquipmentMaintainScheduleResponse>(schedule);

        return scheduleResponse;
    }
}
