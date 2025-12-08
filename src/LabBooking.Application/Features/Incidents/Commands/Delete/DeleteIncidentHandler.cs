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
        // LƯU Ý: Repository cần Include(i => i.IncidentEquipments).ThenInclude(ie => ie.Equipment)
        var incident = await incidentRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Incident), request.Id.ToString());

        // 3. CHECK QUYỀN
        bool isReporter = incident.ReportedById == currentUserId;
        bool isManager = incident.LabRoom?.MainManagerId == currentUserId;

        if (!isAdmin && !isManager && !isReporter)
        {
            logger.LogWarning("User {User} cố xóa Incident {Id} nhưng không có quyền.", currentUserId, request.Id);
            throw new ForbidException("Bạn không có quyền xóa báo cáo sự cố này.");
        }

        // 4. RULE: Không được xóa sự cố đã xử lý xong
        if (incident.IsResolved)
        {
            throw new BadRequestException("Sự cố này đã được xử lý xong. Không thể xóa (Cần giữ lại làm lịch sử).");
        }

        // 5. [SỬA ĐỔI] RULE: Kiểm tra xem có thiết bị nào đang Bảo trì không
        // Lặp qua danh sách IncidentEquipments để kiểm tra
        var maintainingDevice = incident.IncidentEquipments
            .Select(ie => ie.Equipment)
            .FirstOrDefault(e => e.Status == EquipmentStatus.Maintain);

        if (maintainingDevice != null)
        {
            throw new BadRequestException($"Thiết bị '{maintainingDevice.EquipmentName}' đang được bảo trì. Vui lòng hủy lịch bảo trì trước khi xóa báo cáo.");
        }

        // 6. [SỬA ĐỔI] RULE: Hoàn trả trạng thái thiết bị (Nếu là lỗi thiết bị)
        if (incident.Type == IncidentType.EquipmentFailure && incident.IncidentEquipments.Any())
        {
            foreach (var incidentEq in incident.IncidentEquipments)
            {
                var equipment = incidentEq.Equipment;

                // Nếu thiết bị đang bị đánh dấu là Hỏng (Broken), trả về Sẵn sàng (Available)
                if (equipment != null && equipment.Status == EquipmentStatus.Broken)
                {
                    equipment.Status = EquipmentStatus.Available;
                    equipment.IsAvailable = true;

                    // Update từng thiết bị
                    await equipmentRepository.UpdateAsync(equipment, cancellationToken);

                    logger.LogInformation("Restore: Thiết bị {Name} đã được trả về Available do xóa báo cáo hỏng.", equipment.EquipmentName);
                }
            }
        }

        // 7. Xóa Incident
        // EF Core sẽ tự động xóa các dòng con trong bảng IncidentEquipment (Cascade Delete)
        await incidentRepository.DeleteAsync(incident, cancellationToken);

        logger.LogInformation("Deleted: Incident {Id} đã bị xóa bởi {User}.", request.Id, currentUserId);

        return true;
    }
}
