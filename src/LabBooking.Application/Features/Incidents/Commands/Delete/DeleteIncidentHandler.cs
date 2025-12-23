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
        // 1. Lấy thông tin người dùng và vai trò
        var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập.");
        var roles = currentUserService.Roles.ToList();

        // 2. Lấy Incident và các thông tin chi tiết cần thiết
        var incident = await incidentRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Incident), request.Id.ToString());

        // --- BẮT ĐẦU LOGIC PHÂN QUYỀN VÀ KIỂM TRA ĐIỀU KIỆN ---

        bool isManagerOfLab = incident.LabRoom?.MainManagerId == currentUserId;
        bool isReporter = incident.ReportedById == currentUserId;

        if (roles.Contains("Admin"))
        {
            // Admin có toàn quyền, không cần kiểm tra thêm.
        }
        else if (roles.Contains("Manager") && isManagerOfLab)
        {
            // Manager của phòng lab đó có quyền xóa, không giới hạn thời gian.
        }
        else if (roles.Contains("SecurityGuard") && isReporter)
        {
            // Bảo vệ là người tạo sự cố -> Kiểm tra thêm điều kiện thời gian.
            var timeElapsed = DateTime.UtcNow - incident.CreatedAt;
            if (timeElapsed.TotalHours > 24)
            {
                // Đã quá 24 giờ, không cho phép xóa.
                throw new BadRequestException("Bảo vệ chỉ có thể xóa sự cố trong vòng 24 giờ sau khi tạo.");
            }
        }
        else
        {
            // Nếu không rơi vào các trường hợp trên, người dùng không có quyền.
            logger.LogWarning("User {User} không có quyền xóa Incident {Id}.", currentUserId, request.Id);
            throw new ForbiddenAccessException("Bạn không có quyền xóa báo cáo sự cố này.");
        }

        // 4. VALIDATE LOGIC NGHIỆP VỤ (Sau khi đã xác nhận có quyền)
        if (incident.IsResolved)
        {
            throw new BadRequestException("Sự cố đã được xử lý xong, không thể xóa.");
        }

        var maintainingDevice = incident.IncidentEquipments
            .Select(ie => ie.Equipment)
            .FirstOrDefault(e => e != null && e.Status == EquipmentStatus.Maintain);

        if (maintainingDevice != null)
        {
            throw new BadRequestException($"Thiết bị '{maintainingDevice.EquipmentName}' đang được bảo trì. Không thể xóa báo cáo.");
        }

        // 5. GỌI REPO ĐỂ XỬ LÝ
        await incidentRepository.SoftDeleteWithRestoreDevicesAsync(incident, cancellationToken);

        logger.LogInformation("Deleted (Soft): Incident {Id} bởi {User}.", request.Id, currentUserId);
        return true;
    }
}