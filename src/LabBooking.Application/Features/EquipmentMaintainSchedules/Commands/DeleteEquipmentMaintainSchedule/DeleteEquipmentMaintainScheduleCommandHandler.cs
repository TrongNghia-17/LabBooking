using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.DeleteEquipmentMaintainSchedule;

public class DeleteEquipmentMaintainScheduleCommandHandler(
    ILogger<DeleteEquipmentMaintainScheduleCommandHandler> logger,
    IEquipmentMaintainScheduleRepository equipmentMaintainScheduleRepository,
    IEquipmentRepository equipmentRepository,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<DeleteEquipmentMaintainScheduleCommand, Unit>
{
    public async Task<Unit> Handle(DeleteEquipmentMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang xóa EquipmentMaintainSchedule {ScheduleId}", request.Id);

        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập để thực hiện chức năng này.");

        var scheduleToDelete = await equipmentMaintainScheduleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (scheduleToDelete == null)
        {
            logger.LogWarning("EquipmentMaintainSchedule with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(EquipmentMaintainSchedule), request.Id.ToString());
        }

        if (scheduleToDelete.EquimentpMaintainStatus == EquimentpMaintainStatus.Done)
            throw new BadRequestException("Không thể xóa lịch sử bảo trì đã hoàn thành (Done).");

        var equipment = await equipmentRepository.GetByIdAsync(scheduleToDelete.EquipmentId)
            ?? throw new NotFoundException(nameof(Equipment), scheduleToDelete.EquipmentId.ToString());

        var labRoom = await labRoomRepository.GetByIdAsync(equipment.LabRoomId, cancellationToken)
            ?? throw new NotFoundException(nameof(LabRoom), equipment.LabRoomId.ToString());

        if (labRoom.MainManagerId.GetValueOrDefault() != currentUserId)
            throw new ForbidException("Bạn không có quyền xóa lịch bảo trì của thiết bị này.");

        if (equipment.Status == EquipmentStatus.Maintain)
        {
            equipment.Status = EquipmentStatus.Available;
            equipment.IsAvailable = true;

            await equipmentRepository.Update(equipment);

            logger.LogInformation("Đã hoàn trả trạng thái Available cho thiết bị {EqId} do xóa lịch bảo trì.", equipment.Id);
        }

        await equipmentMaintainScheduleRepository.DeleteAsync(scheduleToDelete, cancellationToken);

        return Unit.Value;
    }
}