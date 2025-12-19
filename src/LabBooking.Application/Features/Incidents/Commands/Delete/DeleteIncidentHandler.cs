using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.Incidents.Commands.Delete;

public class DeleteIncidentHandler(
    IIncidentRepository incidentRepository,
    ICurrentUserService currentUserService,
    ILogger<DeleteIncidentHandler> logger
    ) : IRequestHandler<DeleteIncidentCommand, bool>
{
    public async Task<bool> Handle(DeleteIncidentCommand request, CancellationToken cancellationToken)
    {
        // 1. Check Login
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");
        var roles = currentUserService.Roles ?? new List<string>();
        bool isAdmin = roles.Contains("Admin");

        // 2. Lấy Incident
        var incident = await incidentRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (incident == null) throw new NotFoundException(nameof(Incident), request.Id.ToString());

        // 3. CHECK QUYỀN (Admin, Manager phòng đó, hoặc Người báo cáo)
        bool isReporter = incident.ReportedById == currentUserId;
        bool isManager = incident.LabRoom?.MainManagerId == currentUserId;

        if (!isAdmin && !isManager && !isReporter)
        {
            logger.LogWarning("User {User} không có quyền xóa Incident {Id}.", currentUserId, request.Id);
            throw new ForbiddenAccessException("Bạn không có quyền xóa báo cáo sự cố này.");
        }

        // 4. VALIDATE LOGIC
        if (incident.IsResolved)
        {
            throw new BadRequestException("Sự cố đã xử lý xong, không thể xóa.");
        }

        // Check xem có thiết bị nào đang bảo trì không
        var maintainingDevice = incident.IncidentEquipments
            .Select(ie => ie.Equipment)
            .FirstOrDefault(e => e != null && e.Status == EquipmentStatus.Maintain);

        if (maintainingDevice != null)
        {
            throw new BadRequestException($"Thiết bị '{maintainingDevice.EquipmentName}' đang bảo trì. Không thể xóa báo cáo.");
        }

        // 5. GỌI REPO ĐỂ XỬ LÝ (Transaction nằm trong Repo)
        await incidentRepository.SoftDeleteWithRestoreDevicesAsync(incident, cancellationToken);

        logger.LogInformation("Deleted (Soft): Incident {Id} bởi {User}.", request.Id, currentUserId);
        return true;
    }
}