using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Interfaces.Notifications;
using System.Text.Json;

namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentHandler(
    IIncidentRepository incidentRepository,
    IRoomCheckRepository roomCheckRepository, // [THAY THẾ] Dùng Repo thay vì DbContext
    IEquipmentRepository equipmentRepository,
    ICurrentUserService currentUserService,
    INotificationRepository notificationRepository,
    INotificationService notificationService,
    IUserDeviceRepository userDeviceRepository,
    ILogger<CreateIncidentHandler> logger
    ) : IRequestHandler<CreateIncidentCommand, Guid>
{
    public async Task<Guid> Handle(CreateIncidentCommand request, CancellationToken cancellationToken)
    {
        var reporterId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        // 1. Lấy thông tin RoomCheck & Validate Logic "Đạt"
        var roomCheck = await roomCheckRepository.GetByIdWithLabRoomAsync(request.FromRoomCheckId, cancellationToken);
        if (roomCheck == null) throw new NotFoundException("RoomCheck", request.FromRoomCheckId.ToString());

        if (roomCheck.IsPassed)
        {
            throw new BadRequestException("Không thể tạo sự cố từ phiếu kiểm tra 'Đạt'.");
        }

        var labRoomId = roomCheck.LabRoomId;
        var labName = roomCheck.LabRoom?.LabName ?? "Phòng Lab";
        var managerId = roomCheck.LabRoom?.MainManagerId;

        // =========================================================================
        // BƯỚC LỌC THIẾT BỊ (LOGIC QUAN TRỌNG)
        // =========================================================================

        var devicesToAdd = new List<IncidentEquipment>(); // Danh sách thiết bị sẽ đưa vào Incident này
        var updatedEquipments = new List<Equipment>();    // Danh sách cần update DB

        if (request.Type == IncidentType.EquipmentFailure && request.EquipmentIds != null)
        {
            var distinctIds = request.EquipmentIds.Distinct().ToList();
            var equipmentsInDb = await equipmentRepository.GetByIdsAsync(distinctIds, cancellationToken);

            foreach (var equipment in equipmentsInDb)
            {
                // Security Check
                if (equipment.LabRoomId != labRoomId) continue;

                // [LOGIC CHỐNG TRÙNG LẶP]
                if (equipment.Status == EquipmentStatus.Broken)
                {
                    // Nếu thiết bị ĐÃ HỎNG từ trước -> Bỏ qua, không thêm vào Incident mới này.
                    // Vì nó đã thuộc về một Incident cũ nào đó rồi.
                    continue;
                }

                // Nếu thiết bị đang Tốt (Available) -> Giờ mới hỏng
                // 1. Update Status -> Broken
                equipment.Status = EquipmentStatus.Broken;
                equipment.IsAvailable = false;

                // 2. Thêm vào list update
                updatedEquipments.Add(equipment);

                // 3. Chuẩn bị data cho bảng nối IncidentEquipment
                devicesToAdd.Add(new IncidentEquipment
                {
                    Id = Guid.NewGuid(),
                    EquipmentId = equipment.Id
                    // IncidentId sẽ gán sau khi tạo Incident object
                });
            }
        }

        // =========================================================================
        // KIỂM TRA: NẾU KHÔNG CÓ THIẾT BỊ NÀO MỚI HỎNG
        // =========================================================================
        if (request.Type == IncidentType.EquipmentFailure && devicesToAdd.Count == 0)
        {
            // Trường hợp: Check-in hỏng A, B. Check-out lại chọn A, B.
            // Hệ thống lọc ra thấy A, B đều đã Broken -> List rỗng.
            throw new BadRequestException(
                "Các thiết bị bạn chọn ĐÃ ĐƯỢC BÁO HỎNG trước đó rồi. " +
                "Không cần tạo thêm báo cáo trùng lặp.");
        }

        // 2. Tạo Incident (Chỉ chứa các thiết bị MỚI hỏng)
        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            LabRoomId = labRoomId,
            ReportedById = reporterId,
            RoomCheckId = request.FromRoomCheckId,
            Type = request.Type,
            ImportanceLevel = request.ImportanceLevel,
            Description = request.Description, // Giữ nguyên mô tả người dùng nhập
            CreatedAt = DateTime.UtcNow,
            IsResolved = false,
            IncidentEquipments = devicesToAdd // Gán danh sách đã lọc
        };

        // Gán IncidentId ngược lại cho các item con
        foreach (var item in devicesToAdd)
        {
            item.IncidentId = incident.Id;
        }

        // 3. Update trạng thái các thiết bị mới hỏng
        if (updatedEquipments.Any())
        {
            // Nếu Repo của bạn chưa có hàm UpdateRange, hãy dùng loop UpdateAsync
            // Hoặc tốt nhất thêm hàm UpdateRange vào Repo
            foreach (var eq in updatedEquipments)
            {
                await equipmentRepository.UpdateAsync(eq, cancellationToken);
            }
        }

        // 4. Lưu Incident
        await incidentRepository.CreateAsync(incident, cancellationToken);

        // 5. Gửi thông báo
        if (managerId.HasValue)
        {
            await NotifyManagerAsync(managerId.Value, labName, incident, cancellationToken);
        }

        return incident.Id;
    }

    private async Task NotifyManagerAsync(Guid managerId, string labName, Incident incident, CancellationToken token)
    {
        try
        {
            string title = "⚠️ Báo cáo sự cố mới";
            string body = $"Bảo vệ vừa báo cáo sự cố tại {labName}: {incident.Description}";

            var dataPayload = new { incidentId = incident.Id, type = "incident_created", labId = incident.LabRoomId };

            // Lưu Notification
            await notificationRepository.CreateAsync(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = managerId,
                Title = title,
                Message = body,
                DataPayload = JsonSerializer.Serialize(dataPayload),
                CreatedAt = DateTime.UtcNow
            }, token);

            // Gửi Push
            var tokens = await userDeviceRepository.GetTokensByUserIdAsync(managerId, token);
            if (tokens != null && tokens.Any())
                await notificationService.SendPushNotificationAsync(tokens, title, body, dataPayload);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi gửi thông báo Incident");
        }
    }
}