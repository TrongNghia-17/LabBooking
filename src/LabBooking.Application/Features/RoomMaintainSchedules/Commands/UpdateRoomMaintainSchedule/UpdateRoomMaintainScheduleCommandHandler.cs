namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.UpdateRoomMaintainSchedule;

public class UpdateRoomMaintainScheduleCommandHandler(
    ILogger<UpdateRoomMaintainScheduleCommandHandler> logger,
    IMapper mapper,
    IRoomMaintainScheduleRepository roomMaintainScheduleRepository
    ) : IRequestHandler<UpdateRoomMaintainScheduleCommand, Unit>
{
    public async Task<Unit> Handle(UpdateRoomMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang cập nhật RoomMaintainSchedule {ScheduleId}", request.Id);

        // 1. Lấy entity từ repository
        var scheduleToUpdate = await roomMaintainScheduleRepository.GetByIdAsync(request.Id, cancellationToken);

        // 2. Kiểm tra NotFound (giống UpdateLabRoomCommandHandler)
        if (scheduleToUpdate == null)
        {
            logger.LogWarning("RoomMaintainSchedule with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(RoomMaintainSchedule), request.Id.ToString());
        }

        // 3. Map các thay đổi từ Command (request) vào entity
        mapper.Map(request, scheduleToUpdate);

        // 4. Lưu thay đổi
        await roomMaintainScheduleRepository.Update(scheduleToUpdate, cancellationToken);

        return Unit.Value;
    }
}
