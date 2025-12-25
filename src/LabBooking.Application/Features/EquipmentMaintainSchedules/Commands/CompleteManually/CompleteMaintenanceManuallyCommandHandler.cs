using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CompleteManually;

public class CompleteMaintenanceManuallyCommandHandler(
    IEquipmentMaintainScheduleRepository repository,
    ICurrentUserService currentUserService,
    ILogger<CompleteMaintenanceManuallyCommandHandler> logger
) : IRequestHandler<CompleteMaintenanceManuallyCommand, Unit>
{
    public async Task<Unit> Handle(CompleteMaintenanceManuallyCommand request, CancellationToken cancellationToken)
    {
        var managerId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập với vai trò Manager.");

        // 1. Lấy lịch trình và các chi tiết liên quan
        var schedule = await repository.GetByIdWithDetailsAsync(request.ScheduleId, cancellationToken)
            ?? throw new NotFoundException(nameof(EquipmentMaintainSchedule), request.ScheduleId.ToString());

        // 2. KIỂM TRA QUYỀN: Đảm bảo Manager này có quyền trên lịch trình này
        // Logic này dựa trên việc kiểm tra xem có thiết bị nào trong lịch trình thuộc phòng do Manager quản lý hay không.
        bool isOwner = schedule.Details.Any(d => d.Equipment?.LabRoom?.MainManagerId == managerId);
        if (!isOwner)
        {
            logger.LogWarning("Forbidden Access: Manager {ManagerId} tried to complete schedule {ScheduleId} without permission.", managerId, request.ScheduleId);
            throw new ForbiddenAccessException("Bạn không có quyền thực hiện hành động này trên lịch trình đã chọn.");
        }

        // 3. Kiểm tra xem lịch có đang ở trạng thái có thể hoàn thành không
        if (schedule.Status == MaintenanceStatus.Done)
        {
            throw new InvalidOperationException("Lịch trình này đã được hoàn thành trước đó.");
        }

        // 4. Gọi phương thức repository để thực hiện logic
        await repository.CompleteScheduleManuallyAsync(schedule, cancellationToken);

        logger.LogInformation("Manager {ManagerId} manually completed maintenance schedule {ScheduleId}.", managerId, request.ScheduleId);

        return Unit.Value;
    }
}
