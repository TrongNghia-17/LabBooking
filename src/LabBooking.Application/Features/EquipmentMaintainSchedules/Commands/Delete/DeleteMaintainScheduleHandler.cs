using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.Delete;

public class DeleteMaintainScheduleHandler(
    IEquipmentMaintainScheduleRepository repository,
    ICurrentUserService currentUserService,
    ILogger<DeleteMaintainScheduleHandler> logger
    ) : IRequestHandler<DeleteMaintainScheduleCommand, bool>
{
    public async Task<bool> Handle(DeleteMaintainScheduleCommand request, CancellationToken cancellationToken)
    {
        // 1. Check Đăng nhập
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");

        var roles = currentUserService.Roles.ToList();
        bool isAdmin = roles.Contains("Admin");

        // 2. Lấy dữ liệu từ DB
        var schedule = await repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (schedule == null)
            throw new NotFoundException(nameof(EquipmentMaintainSchedule), request.Id.ToString());

        // 3. Check Quyền (Security Check)
        // Lấy thiết bị đầu tiên để xác định phòng Lab
        var firstDetail = schedule.Details.FirstOrDefault();
        var labRoom = firstDetail?.Equipment?.LabRoom;

        // Nếu không phải Admin VÀ (Phòng null HOẶC ManagerId không khớp) -> Chặn
        if (!isAdmin && labRoom?.MainManagerId != currentUserId)
        {
            logger.LogWarning("Security: User {User} cố xóa lịch {Sch} của phòng khác.", currentUserId, request.Id);
            throw new ForbidException("Bạn không quản lý phòng Lab này nên không được xóa lịch.");
        }

        // 4. Rule: Không được xóa lịch sử đã hoàn thành
        if (schedule.Status == MaintenanceStatus.Done)
        {
            throw new BadRequestException("Lịch bảo trì đã hoàn tất (Done). Không thể xóa dữ liệu lịch sử.");
        }

        // 5. Rule QUAN TRỌNG: Giải phóng thiết bị (Revert Status)
        // Nếu xóa lịch mà không trả thiết bị về Available -> Thiết bị sẽ bị kẹt vĩnh viễn.
        foreach (var detail in schedule.Details)
        {
            if (detail.Equipment != null && detail.Equipment.Status == EquipmentStatus.Maintain)
            {
                detail.Equipment.Status = EquipmentStatus.Available;
                detail.Equipment.IsAvailable = true;

                logger.LogInformation("Restore: Thiết bị '{Name}' đã được mở khóa (Available) do hủy lịch.", detail.Equipment.EquipmentName);
            }
        }

        // 6. Xóa và Lưu
        await repository.DeleteAsync(schedule, cancellationToken);

        logger.LogInformation("Deleted: Lịch bảo trì {Id} đã bị xóa thành công.", request.Id);
        return true;
    }
}
