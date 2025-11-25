namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.AutoUpdateStatus;

public class AutoUpdateRoomMaintainStatusCommandHandler(
    ILogger<AutoUpdateRoomMaintainStatusCommandHandler> logger,
    IRoomMaintainScheduleRepository roomMaintainScheduleRepository
    ) : IRequestHandler<AutoUpdateRoomMaintainStatusCommand, int>
{
    public async Task<int> Handle(AutoUpdateRoomMaintainStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("CRON JOB INITIATED: Bắt đầu kiểm tra và cập nhật lịch bảo trì hết hạn.");

        // 1. Lấy danh sách lịch hết hạn và chưa Done
        var expiredSchedules = await roomMaintainScheduleRepository.GetExpiredNotYetSchedulesAsync(cancellationToken);

        if (!expiredSchedules.Any())
        {
            logger.LogInformation("CRON JOB COMPLETE: Không tìm thấy lịch bảo trì nào cần cập nhật.");
            return 0;
        }

        // 2. Cập nhật trạng thái sang Done và lưu DB
        await roomMaintainScheduleRepository.UpdateRange(expiredSchedules, cancellationToken);

        logger.LogInformation("CRON JOB COMPLETE: Đã cập nhật thành công {Count} lịch bảo trì sang trạng thái Done.", expiredSchedules.Count());

        return expiredSchedules.Count();
    }
}
