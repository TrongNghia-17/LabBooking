using LabBooking.Application.Common.Interfaces;

namespace LabBooking.Application.Features.RoomChecks.Commands.CreateRoomCheck;

public class CreateRoomCheckCommandHandler(
    IRoomCheckRepository roomCheckRepository,
    IEquipmentRepository equipmentRepository,
    ILabRoomRepository labRoomRepository, // Để check phòng tồn tại
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IRequestHandler<CreateRoomCheckCommand, Guid>
{
    public async Task<Guid> Handle(CreateRoomCheckCommand request, CancellationToken cancellationToken)
    {
        // 1. Validation & Data Preparation
        var guardId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var labExists = await labRoomRepository.GetByIdAsync(request.LabRoomId, cancellationToken);
        if (labExists == null) throw new KeyNotFoundException($"Lab ID {request.LabRoomId} not found.");

        // Load thiết bị từ DB để tracking và update status
        var equipmentIds = request.EquipmentDetails.Select(x => x.EquipmentId).Distinct().ToList();
        var equipmentsInDb = await equipmentRepository.GetByIdsAsync(equipmentIds, cancellationToken);

        // 2. Mapping (Command -> Entity)
        // AutoMapper tự tạo RoomCheck và List<EquipmentCheckResult>
        var roomCheck = mapper.Map<RoomCheck>(request);
        roomCheck.GuardId = guardId; // Gán ID người dùng

        // 3. Business Logic Loop (Xử lý Incident & Update Status)
        foreach (var detail in roomCheck.Details)
        {
            var equipment = equipmentsInDb.FirstOrDefault(e => e.Id == detail.EquipmentId);

            // Security Check: Thiết bị phải thuộc phòng Lab này
            if (equipment == null || equipment.LabRoomId != request.LabRoomId)
            {
                // Logic tùy chọn: Remove detail sai lệch hoặc Ignore
                continue;
            }

            // Nếu thiết bị HỎNG -> Tạo Incident
            // Lưu ý: detail.IsOK đã được map từ DTO
            if (!detail.IsOK)
            {
                var issueDesc = detail.IssueDescription ?? "Hư hỏng không xác định";

                // Tạo Incident Entity
                var incident = CreateIncident(request.LabRoomId, guardId, equipment.Id, request.Type, issueDesc);

                // Link Incident vào Detail (EF Core sẽ tự Insert Incident này khi lưu RoomCheck)
                detail.Incident = incident;

                // Update trạng thái thiết bị (Entity đang được Track bởi EF Core qua Repo)
                equipment.Status = EquipmentStatus.Broken;
                equipment.IsAvailable = false;
            }
        }

        // 4. Save to Database
        // Repo sẽ lưu RoomCheck, Details, Incidents (qua navigation property) và update Equipment
        await roomCheckRepository.AddAsync(roomCheck, cancellationToken);

        return roomCheck.Id;
    }

    // Tách hàm tạo Incident cho gọn code
    private static Incident CreateIncident(Guid labId, Guid reporterId, Guid equipId, CheckType type, string desc)
    {
        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            LabRoomId = labId,
            ReportedById = reporterId,
            Type = IncidentType.EquipmentFailure,
            ImportanceLevel = LevelOfImportance.High,
            Description = $"[Auto-Report: {type}] {desc}",
            CreatedAt = DateTime.UtcNow,
            IsResolved = false,
            IncidentEquipments = new List<IncidentEquipment>()
        };

        incident.IncidentEquipments.Add(new IncidentEquipment
        {
            Id = Guid.NewGuid(),
            IncidentId = incident.Id,
            EquipmentId = equipId
        });

        return incident;
    }
}