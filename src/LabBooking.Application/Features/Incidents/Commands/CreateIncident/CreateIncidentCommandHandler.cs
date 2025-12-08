using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentHandler(
    IIncidentRepository incidentRepository,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService,
    ILogger<CreateIncidentHandler> logger,
    IEquipmentRepository equipmentRepository,
    IMapper mapper
    ) : IRequestHandler<CreateIncidentCommand, Guid>
{
    public async Task<Guid> Handle(CreateIncidentCommand request, CancellationToken cancellationToken)
    {
        // 1. Check Login & Phòng Lab (Giữ nguyên)
        var reporterId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        if (!await labRoomRepository.ExistsAsync(request.LabRoomId, cancellationToken))
            throw new NotFoundException(nameof(LabRoom), request.LabRoomId.ToString());

        // 2. Tạo Object INCIDENT (Cha)
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
            // Khởi tạo danh sách con trống
            IncidentEquipments = new List<IncidentEquipment>()
        };

        // 3. Xử lý danh sách Thiết bị (Con)
        if (request.Type == IncidentType.EquipmentFailure && request.EquipmentIds != null)
        {
            // Lọc trùng ID
            var distinctIds = request.EquipmentIds.Distinct();

            foreach (var eqId in distinctIds)
            {
                // Lấy thiết bị để update trạng thái
                var equipment = await equipmentRepository.GetByIdAsync(eqId);

                // Validator đã check rồi, nhưng check lại cho an toàn logic
                if (equipment != null && equipment.LabRoomId == request.LabRoomId)
                {
                    // a. Cập nhật trạng thái thiết bị -> Hỏng
                    if (equipment.Status != EquipmentStatus.Broken)
                    {
                        equipment.Status = EquipmentStatus.Broken;
                        equipment.IsAvailable = false;
                        await equipmentRepository.UpdateAsync(equipment, cancellationToken);
                    }

                    // b. Tạo dòng chi tiết IncidentEquipment
                    var detail = new IncidentEquipment
                    {
                        Id = Guid.NewGuid(),
                        IncidentId = incident.Id, // Link với cha
                        EquipmentId = eqId
                    };

                    // Add vào list của cha
                    incident.IncidentEquipments.Add(detail);
                }
            }
        }

        // 4. Lưu vào Database (EF Core sẽ tự lưu cả Cha và các Con)
        await incidentRepository.CreateAsync(incident, cancellationToken);

        logger.LogInformation("Đã tạo sự cố {IncidentId} gồm {Count} thiết bị.", incident.Id, incident.IncidentEquipments.Count);

        // 5. Trả về ID của Incident duy nhất này
        return incident.Id;
    }
}
