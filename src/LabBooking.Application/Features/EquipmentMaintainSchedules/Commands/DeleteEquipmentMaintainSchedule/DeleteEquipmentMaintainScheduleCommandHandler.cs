namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.DeleteEquipmentMaintainSchedule;

public class DeleteEquipmentMaintainScheduleCommandHandler(
    ILogger<DeleteEquipmentMaintainScheduleCommandHandler> logger,
    IEquipmentMaintainScheduleRepository equipmentMaintainScheduleRepository
    ) : IRequestHandler<DeleteEquipmentMaintainScheduleCommand, Unit>
{
    public async Task<Unit> Handle(DeleteEquipmentMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang xóa EquipmentMaintainSchedule {ScheduleId}", request.Id);

        // 1. Lấy entity từ repository
        var scheduleToDelete = await equipmentMaintainScheduleRepository.GetByIdAsync(request.Id, cancellationToken);

        // 2. Kiểm tra NotFound (giống DeleteLabRoomCommandHandler)
        if (scheduleToDelete == null)
        {
            logger.LogWarning("EquipmentMaintainSchedule with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(EquipmentMaintainSchedule), request.Id.ToString());
        }

        // 3. Xóa
        await equipmentMaintainScheduleRepository.DeleteAsync(scheduleToDelete, cancellationToken);

        return Unit.Value;
    }
}