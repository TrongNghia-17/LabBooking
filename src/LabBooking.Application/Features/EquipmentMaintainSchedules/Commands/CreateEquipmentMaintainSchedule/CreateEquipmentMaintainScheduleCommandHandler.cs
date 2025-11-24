
namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;

public class CreateEquipmentMaintainScheduleCommandHandler(
    ILogger<CreateEquipmentMaintainScheduleCommandHandler> logger,
    IMapper mapper,
    IEquipmentMaintainScheduleRepository equipmentMaintainScheduleRepository // Repository mới
    ) : IRequestHandler<CreateEquipmentMaintainScheduleCommand, Guid>
{
    public async Task<Guid> Handle(CreateEquipmentMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang tạo một EquipmentMaintainSchedule mới cho Equipment {EquipmentId}", request.EquipmentId);

        // 1. Map từ Command sang Entity
        var schedule = mapper.Map<EquipmentMaintainSchedule>(request);

        // 2. Gán các giá trị mặc định
        schedule.Id = Guid.NewGuid();
        schedule.EquimentpMaintainStatus = EquimentpMaintainStatus.NotYet; // Gán trạng thái mặc định

        // 3. Lưu vào database
        var scheduleId = await equipmentMaintainScheduleRepository.Create(schedule, cancellationToken);

        return scheduleId;
    }
}