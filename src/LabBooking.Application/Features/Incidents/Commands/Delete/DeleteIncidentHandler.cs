using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.Incidents.Commands.Delete;

public class DeleteIncidentHandler(
    IIncidentRepository incidentRepository,
    IEquipmentRepository equipmentRepository,
    ICurrentUserService currentUserService,
    ILogger<DeleteIncidentHandler> logger
    ) : IRequestHandler<DeleteIncidentCommand, bool>
{
    public async Task<bool> Handle(DeleteIncidentCommand request, CancellationToken cancellationToken)
    {
        // 1. Check Login
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");
        var roles = currentUserService.Roles.ToList();
        bool isAdmin = roles.Contains("Admin");

        // 2. Lấy Incident từ DB
        var incident = await incidentRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Incident), request.Id.ToString());

        // 3. CHECK QUYỀN (Logic cực kỳ quan trọng)
        // Quyền 1: Chính chủ (Reporter) được xóa.
        bool isReporter = incident.ReportedById == currentUserId;

        // Quyền 2: Manager của phòng Lab đó được xóa.
        bool isManager = incident.LabRoom?.MainManagerId == currentUserId;

        // Nếu không phải Admin, không phải Chủ phòng, cũng không phải Người báo -> Cấm
        if (!isAdmin && !isManager && !isReporter)
        {
            logger.LogWarning("User {User} cố xóa Incident {Id} nhưng không có quyền.", currentUserId, request.Id);
            throw new ForbidException("Bạn không có quyền xóa báo cáo sự cố này.");
        }

        // --- LOGIC MỚI: CHẶN XÓA NẾU ĐANG BẢO TRÌ ---
        if (incident.Equipment != null && incident.Equipment.Status == EquipmentStatus.Maintain)
        {
            throw new BadRequestException("Thiết bị này đang được bảo trì. Vui lòng hủy lịch bảo trì trước khi xóa báo cáo sự cố.");
        }

        // 4. RULE: Không được xóa sự cố đã xử lý xong (Resolved)
        if (incident.IsResolved)
        {
            throw new BadRequestException("Sự cố này đã được xử lý xong. Không thể xóa (Cần giữ lại làm lịch sử).");
        }

        // 5. RULE: Hoàn trả trạng thái Thiết bị (Nếu là báo hỏng nhầm)
        // Nếu sự cố là 'Hư hỏng' VÀ có gắn thiết bị VÀ thiết bị đang bị set là Broken
        if (incident.Type == IncidentType.EquipmentFailure &&
            incident.Equipment != null &&
            incident.Equipment.Status == EquipmentStatus.Broken)
        {
            incident.Equipment.Status = EquipmentStatus.Available;
            incident.Equipment.IsAvailable = true;

            // Cập nhật lại thiết bị
            await equipmentRepository.UpdateAsync(incident.Equipment, cancellationToken);

            logger.LogInformation("Restore: Thiết bị {Name} đã được trả về Available do xóa báo cáo hỏng.", incident.Equipment.EquipmentName);
        }

        // 6. Xóa Incident
        await incidentRepository.DeleteAsync(incident, cancellationToken);

        logger.LogInformation("Deleted: Incident {Id} đã bị xóa bởi {User}.", request.Id, currentUserId);
        return true;
    }
}
