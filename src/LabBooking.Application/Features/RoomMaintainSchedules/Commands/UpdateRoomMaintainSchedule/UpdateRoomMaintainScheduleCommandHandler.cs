using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.UpdateRoomMaintainSchedule;

public class UpdateRoomMaintainScheduleCommandHandler(
    ILogger<UpdateRoomMaintainScheduleCommandHandler> logger,
    IMapper mapper,
    IRoomMaintainScheduleRepository roomMaintainScheduleRepository,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<UpdateRoomMaintainScheduleCommand, Unit>
{
    public async Task<Unit> Handle(UpdateRoomMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Đang cập nhật RoomMaintainSchedule {ScheduleId}", request.Id);

        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập để thực hiện chức năng này.");

        var scheduleToUpdate = await roomMaintainScheduleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (scheduleToUpdate == null)
        {
            logger.LogWarning("RoomMaintainSchedule with Id: {Id} not found.", request.Id);
            throw new NotFoundException(nameof(RoomMaintainSchedule), request.Id.ToString());
        }

        var labRoom = await labRoomRepository.GetByIdAsync(scheduleToUpdate.LabRoomId, cancellationToken)
            ?? throw new NotFoundException(nameof(LabRoom), scheduleToUpdate.LabRoomId.ToString());

        if (labRoom.MainManagerId != currentUserId)
        {
            logger.LogWarning("User {UserId} cố gắng update lịch {ScheduleId} nhưng không phải là quản lý của Lab {LabId}.", currentUserId, request.Id, labRoom.Id);
            throw new ForbidException("Bạn không có quyền cập nhật lịch bảo trì này vì bạn không quản lý phòng Lab tương ứng.");
        }

        if (scheduleToUpdate.RoomMaintainStatus == RoomMaintainStatus.Done)
        {
            throw new BadRequestException("Lịch bảo trì này đã hoàn thành (Done). Không thể chỉnh sửa, vui lòng tạo lịch bảo trì mới.");
        }

        mapper.Map(request, scheduleToUpdate);

        await roomMaintainScheduleRepository.Update(scheduleToUpdate, cancellationToken);

        return Unit.Value;
    }
}
