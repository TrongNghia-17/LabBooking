using LabBooking.Application.Services.Notifications;
using LabBooking.Application.Services.Users;
using System.Text.Json;

namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentHandler(
    IIncidentRepository incidentRepository,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService,
    IEquipmentRepository equipmentRepository,
    ILogger<CreateIncidentHandler> logger,
    IMapper mapper,
    // [THÊM MỚI] Inject các service liên quan đến thông báo
    INotificationService notificationService,
    INotificationRepository notificationRepository,
    IUserDeviceRepository userDeviceRepository
    ) : IRequestHandler<CreateIncidentCommand, Guid>
{
    public async Task<Guid> Handle(CreateIncidentCommand request, CancellationToken cancellationToken)
    {
        // 1. Check Login
        var reporterId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        // 2. Lấy thông tin phòng Lab (Cần lấy entity để biết ManagerId)
        var labRoom = await labRoomRepository.GetByIdAsync(request.LabRoomId, cancellationToken);
        if (labRoom == null)
            throw new NotFoundException(nameof(LabRoom), request.LabRoomId.ToString());

        // 3. Logic tạo Incident (Code cũ giữ nguyên)
        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            LabRoomId = request.LabRoomId,
            ReportedById = reporterId,
            Type = request.Type,
            ImportanceLevel = request.ImportanceLevel,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            IsResolved = false,
            IncidentEquipments = new List<IncidentEquipment>()
        };

        // Xử lý thiết bị (Code cũ giữ nguyên)
        if (request.Type == IncidentType.EquipmentFailure && request.EquipmentIds != null)
        {
            var distinctIds = request.EquipmentIds.Distinct();
            foreach (var eqId in distinctIds)
            {
                var equipment = await equipmentRepository.GetByIdAsync(eqId);
                if (equipment != null && equipment.LabRoomId == request.LabRoomId)
                {
                    if (equipment.Status != EquipmentStatus.Broken)
                    {
                        equipment.Status = EquipmentStatus.Broken;
                        equipment.IsAvailable = false;
                        await equipmentRepository.UpdateAsync(equipment, cancellationToken);
                    }
                    incident.IncidentEquipments.Add(new IncidentEquipment
                    {
                        Id = Guid.NewGuid(),
                        IncidentId = incident.Id,
                        EquipmentId = eqId
                    });
                }
            }
        }

        // 4. Lưu Incident vào Database
        await incidentRepository.CreateAsync(incident, cancellationToken);
        logger.LogInformation("Đã tạo sự cố {IncidentId}", incident.Id);

        // ========================================================================
        // [THÊM MỚI] GỬI THÔNG BÁO CHO MANAGER
        // ========================================================================
        try
        {
            // Kiểm tra xem phòng có Manager không
            if (labRoom.MainManagerId.HasValue)
            {
                var managerId = labRoom.MainManagerId.Value;

                // (Tùy chọn) Không gửi thông báo nếu chính Manager là người báo cáo
                if (managerId != reporterId)
                {
                    await SendNotificationToManager(managerId, labRoom.LabName, incident, cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            // Quan trọng: Try-catch để nếu lỗi gửi thông báo thì KHÔNG làm lỗi việc tạo sự cố
            logger.LogError(ex, "Lỗi khi gửi thông báo cho Manager phòng {Lab}", labRoom.LabName);
        }
        // ========================================================================

        return incident.Id;
    }

    // Hàm tách riêng để xử lý logic gửi thông báo cho gọn
    private async Task SendNotificationToManager(
        Guid managerId,
        string labName,
        Incident incident,
        CancellationToken token)
    {
        // A. Chuẩn bị nội dung
        string title = "⚠️ Báo cáo sự cố mới";
        string body = $"Phòng {labName} vừa có báo cáo sự cố: {incident.Description}";

        // Dữ liệu kèm theo (để khi bấm vào thông báo thì mở màn hình chi tiết)
        var dataPayload = new
        {
            incidentId = incident.Id,
            type = "incident_created",
            labId = incident.LabRoomId
        };

        // B. Lưu vào Database (Bảng Notification)
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = managerId, // Người nhận là Manager
            Title = title,
            Message = body,
            DataPayload = JsonSerializer.Serialize(dataPayload),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await notificationRepository.CreateAsync(notification, token);

        // C. Gửi Push Notification (Qua Expo/Firebase)
        // 1. Lấy token của Manager
        var tokens = await userDeviceRepository.GetTokensByUserIdAsync(managerId, token);

        // 2. Gửi nếu có token
        if (tokens != null && tokens.Any())
        {
            await notificationService.SendPushNotificationAsync(tokens, title, body, dataPayload);
            logger.LogInformation("Đã gửi Push Notification đến Manager {ManagerId}", managerId);
        }
    }
}
