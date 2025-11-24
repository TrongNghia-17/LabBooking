namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.UpdateEquipmentMaintainSchedule;

public class UpdateEquipmentMaintainScheduleCommandHandler(
    ILogger<UpdateEquipmentMaintainScheduleCommandHandler> logger,
    IMapper mapper,
    IEquipmentMaintainScheduleRepository equipmentMaintainScheduleRepository
    ) : IRequestHandler<UpdateEquipmentMaintainScheduleCommand, Unit>
{
    public async Task<Unit> Handle(UpdateEquipmentMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang cập nhật EquipmentMaintainSchedule {ScheduleId}", request.Id);

        // 1. Lấy entity từ repository
        var scheduleToUpdate = await equipmentMaintainScheduleRepository.GetByIdAsync(request.Id, cancellationToken);

        // 2. Kiểm tra NotFound (giống UpdateLabRoomCommandHandler)
        if (scheduleToUpdate == null)
        {
            logger.LogWarning("EquipmentMaintainSchedule with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(EquipmentMaintainSchedule), request.Id.ToString());
        }

        // 3. Map các thay đổi từ Command (request) vào entity
        mapper.Map(request, scheduleToUpdate);

        // 4. Lưu thay đổi
        await equipmentMaintainScheduleRepository.Update(scheduleToUpdate, cancellationToken);

        return Unit.Value;
    }
}