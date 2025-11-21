namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.DeleteRoomMaintainSchedule;

public class DeleteRoomMaintainScheduleCommandHandler(
    ILogger<DeleteRoomMaintainScheduleCommandHandler> logger,
    IRoomMaintainScheduleRepository roomMaintainScheduleRepository
    ) : IRequestHandler<DeleteRoomMaintainScheduleCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRoomMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang xóa RoomMaintainSchedule {ScheduleId}", request.Id);

        // 1. Lấy entity từ repository
        var scheduleToDelete = await roomMaintainScheduleRepository.GetByIdAsync(request.Id, cancellationToken);

        // 2. Kiểm tra NotFound (giống DeleteLabRoomCommandHandler)
        if (scheduleToDelete == null)
        {
            logger.LogWarning("RoomMaintainSchedule with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(RoomMaintainSchedule), request.Id.ToString());
        }

        // 3. Xóa
        await roomMaintainScheduleRepository.DeleteAsync(scheduleToDelete, cancellationToken);

        return Unit.Value;
    }
}
