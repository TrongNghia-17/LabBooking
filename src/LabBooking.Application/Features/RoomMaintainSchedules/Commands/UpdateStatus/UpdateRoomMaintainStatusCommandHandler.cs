using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.UpdateStatus
{
    public class UpdateRoomMaintainStatusCommandHandler(
    ILogger<UpdateRoomMaintainStatusCommandHandler> logger,
    IRoomMaintainScheduleRepository repository,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<UpdateRoomMaintainStatusCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateRoomMaintainStatusCommand request, CancellationToken cancellationToken)
        {
            // 1. Check Auth
            var currentUserId = currentUserService.UserId;
            if (currentUserId == null) throw new UnauthorizedAccessException("Cần đăng nhập.");

            // 2. Lấy Lịch
            var schedule = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (schedule == null) throw new NotFoundException(nameof(RoomMaintainSchedule), request.Id.ToString());

            // 3. Check quyền Manager (Logic cũ)
            var labRoom = await labRoomRepository.GetByIdAsync(schedule.LabRoomId, cancellationToken);
            if (labRoom == null || labRoom.MainManagerId != currentUserId)
            {
                throw new ForbidException("Không có quyền cập nhật trạng thái lịch bảo trì này.");
            }

            // 4. Logic nghiệp vụ chuyển trạng thái
            // Ví dụ: Nếu đã Done rồi thì có cho chuyển lại NotYet không?
            // Nếu bạn muốn chặt chẽ: "Đã xong là xong luôn, không quay lại"
            if (schedule.RoomMaintainStatus == RoomMaintainStatus.Done && request.Status == RoomMaintainStatus.NotYet)
            {
                throw new BadRequestException("Không thể chuyển trạng thái từ Đã xong (Done) về Chưa xong (NotYet).");
            }

            // 5. Cập nhật
            schedule.RoomMaintainStatus = request.Status;

            // Nếu bạn có trường CompletedAt (Thời gian hoàn thành)
            // if (request.Status == RoomMaintainStatus.Done) schedule.CompletedAt = DateTime.UtcNow;

            await repository.Update(schedule, cancellationToken);

            logger.LogInformation("Updated status of Schedule {Id} to {Status}", request.Id, request.Status);

            return Unit.Value;
        }
    }
}
