using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.UpdateEquipmentMaintainSchedule;

public class UpdateEquipmentMaintainScheduleCommandHandler(
    ILogger<UpdateEquipmentMaintainScheduleCommandHandler> logger,
    IMapper mapper,
    IEquipmentMaintainScheduleRepository equipmentMaintainScheduleRepository,
    IEquipmentRepository equipmentRepository,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<UpdateEquipmentMaintainScheduleCommand, Unit>
{
    public async Task<Unit> Handle(UpdateEquipmentMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang cập nhật EquipmentMaintainSchedule {ScheduleId}", request.Id);

        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");

        var scheduleToUpdate = await equipmentMaintainScheduleRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(EquipmentMaintainSchedule), request.Id.ToString());

        if (scheduleToUpdate.EquimentpMaintainStatus == EquimentpMaintainStatus.Done)
            throw new BadRequestException("Lịch bảo trì đã hoàn thành (Done). Không được phép chỉnh sửa lịch sử.");

        var equipment = await equipmentRepository.GetByIdAsync(scheduleToUpdate.EquipmentId)
            ?? throw new NotFoundException(nameof(Equipment), scheduleToUpdate.EquipmentId.ToString());

        var labRoom = await labRoomRepository.GetByIdAsync(equipment.LabRoomId, cancellationToken)
            ?? throw new NotFoundException(nameof(LabRoom), equipment.LabRoomId.ToString());

        if (labRoom.MainManagerId.GetValueOrDefault() != currentUserId)
            throw new ForbidException("Bạn không có quyền chỉnh sửa lịch bảo trì của phòng Lab này.");

        bool isTimeChanged = request.StartTime != scheduleToUpdate.StartTime
                      || request.EndTime != scheduleToUpdate.EndTime;

        if (isTimeChanged)
        {
            var isOverlap = await equipmentMaintainScheduleRepository.IsOverlapAsync(
                scheduleToUpdate.EquipmentId,
                request.StartTime,
                request.EndTime,
                scheduleToUpdate.Id,
                cancellationToken);

            if (isOverlap)
                throw new BadRequestException("Thời gian cập nhật bị trùng với một lịch bảo trì khác.");
        }

        mapper.Map(request, scheduleToUpdate);
        await equipmentMaintainScheduleRepository.Update(scheduleToUpdate, cancellationToken);

        return Unit.Value;
    }
}