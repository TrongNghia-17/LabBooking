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
        var guardId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        // 1. Gọi Repo lấy thông tin RoomCheck
        var roomCheck = await roomCheckRepository.GetByIdWithLabRoomAsync(request.FromRoomCheckId, cancellationToken)
            ?? throw new NotFoundException("RoomCheck", request.FromRoomCheckId.ToString());

        // ========================================================================
        // [FIX LOGIC] CHẶN TẠO INCIDENT NẾU PHIẾU CHECK LÀ "TỐT"
        // ========================================================================
        if (roomCheck.IsPassed)
        {
            throw new BadRequestException(
                "Phiếu kiểm tra này đã được đánh giá là 'Đạt' (Tốt). " +
                "Không thể tạo sự cố từ phiếu này. " +
                "Vui lòng xóa phiếu kiểm tra cũ và tạo lại phiếu 'Không đạt' nếu có sự nhầm lẫn.");
        }
        // ========================================================================

        var labRoomId = roomCheck.LabRoomId;
        var labName = roomCheck.LabRoom?.LabName ?? "Phòng Lab";
        var managerId = roomCheck.LabRoom?.MainManagerId;

        // 2. Tạo Incident
        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            LabRoomId = labRoomId,
            ReportedById = guardId,
            RoomCheckId = request.FromRoomCheckId,
            Type = request.Type,
            ImportanceLevel = request.ImportanceLevel,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            IsResolved = false,
            IncidentEquipments = new List<IncidentEquipment>()
        };

        // 3. Xử lý thiết bị hỏng (Nếu có)
        if (request.Type == IncidentType.EquipmentFailure && request.EquipmentIds != null)
        {
            var distinctIds = request.EquipmentIds.Distinct().ToList();
            var equipments = await equipmentRepository.GetByIdsAsync(distinctIds, cancellationToken);

            foreach (var equipment in equipments)
            {
                // Security Check: Thiết bị phải thuộc đúng phòng của cái RoomCheck kia
                if (equipment.LabRoomId != labRoomId) continue;

                // Update Status -> Broken
                if (equipment.Status != EquipmentStatus.Broken)
                {
                    equipment.Status = EquipmentStatus.Broken;
                    equipment.IsAvailable = false;
                    await equipmentRepository.UpdateAsync(equipment, cancellationToken);
                }

                // Add to Incident
                incident.IncidentEquipments.Add(new IncidentEquipment
                {
                    Id = Guid.NewGuid(),
                    IncidentId = incident.Id,
                    EquipmentId = equipment.Id
                });
            }
        }

        // 4. Lưu Incident
        await incidentRepository.CreateAsync(incident, cancellationToken);
        logger.LogInformation("Đã tạo sự cố {IncidentId} từ đợt kiểm tra {CheckId}", incident.Id, request.FromRoomCheckId);

        // 5. Gửi thông báo cho Manager
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