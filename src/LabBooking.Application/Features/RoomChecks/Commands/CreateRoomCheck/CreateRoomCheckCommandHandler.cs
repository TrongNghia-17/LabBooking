using LabBooking.Application.Common.Interfaces;
using System.Text;

namespace LabBooking.Application.Features.RoomChecks.Commands.CreateRoomCheck;

public class CreateRoomCheckCommandHandler(
    IRoomCheckRepository roomCheckRepository,
    IEquipmentRepository equipmentRepository,
    ILabRoomRepository labRoomRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<CreateRoomCheckCommand, Guid>
{
    public async Task<Guid> Handle(CreateRoomCheckCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate & Get Data
        var guardId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var labExists = await labRoomRepository.GetByIdAsync(request.LabRoomId, cancellationToken);
        if (labExists == null) throw new KeyNotFoundException($"Không tìm thấy phòng Lab ID {request.LabRoomId}");

        // Load thiết bị để check logic
        var requestEquipmentIds = request.EquipmentDetails.Select(x => x.EquipmentId).Distinct().ToList();
        var equipmentsInDb = await equipmentRepository.GetByIdsAsync(requestEquipmentIds, cancellationToken);

        // 2. Chuẩn bị dữ liệu (In-Memory)
        var roomCheck = new RoomCheck
        {
            Id = Guid.NewGuid(),
            GuardId = guardId,
            LabRoomId = request.LabRoomId,
            CheckedAt = DateTime.UtcNow,
            Type = request.Type,
            Note = request.Note,
            Details = new List<EquipmentCheckResult>()
        };

        var brokenDetails = new List<EquipmentCheckResult>(); // List tạm để tạo Incident
        var updatedEquipments = new List<Equipment>();        // List tạm để update DB

        // 3. Vòng lặp xử lý logic
        foreach (var itemDto in request.EquipmentDetails)
        {
            var equipment = equipmentsInDb.FirstOrDefault(e => e.Id == itemDto.EquipmentId);

            // Security Check: Thiết bị phải thuộc phòng này
            if (equipment == null || equipment.LabRoomId != request.LabRoomId) continue;

            var checkDetail = new EquipmentCheckResult
            {
                Id = Guid.NewGuid(),
                RoomCheckId = roomCheck.Id,
                EquipmentId = itemDto.EquipmentId,
                IsOK = itemDto.IsOK,
                IssueDescription = itemDto.IssueDescription
            };

            // Nếu HỎNG
            if (!itemDto.IsOK)
            {
                // Update Object trong bộ nhớ
                equipment.Status = EquipmentStatus.Broken;
                equipment.IsAvailable = false;

                updatedEquipments.Add(equipment); // Đưa vào danh sách cần Update
                brokenDetails.Add(checkDetail);   // Đưa vào danh sách cần tạo Incident
            }

            roomCheck.Details.Add(checkDetail);
        }

        // 4. Tạo 1 Incident Duy Nhất (Nếu có đồ hỏng)
        Incident? masterIncident = null;

        if (brokenDetails.Count > 0)
        {
            var descriptionBuilder = new StringBuilder();
            descriptionBuilder.Append($"[Báo cáo {request.Type}]: ");

            foreach (var broken in brokenDetails)
            {
                var eqName = equipmentsInDb.First(e => e.Id == broken.EquipmentId).EquipmentName;
                descriptionBuilder.Append($"{eqName} ({broken.IssueDescription}); ");
            }

            masterIncident = new Incident
            {
                Id = Guid.NewGuid(),
                LabRoomId = request.LabRoomId,
                ReportedById = guardId,
                Type = IncidentType.EquipmentFailure,
                ImportanceLevel = LevelOfImportance.High,
                Description = descriptionBuilder.ToString(),
                CreatedAt = DateTime.UtcNow,
                IsResolved = false,
                IncidentEquipments = new List<IncidentEquipment>()
            };

            // Link Incident <-> Equipment <-> Detail
            foreach (var brokenDetail in brokenDetails)
            {
                // Link bảng trung gian
                masterIncident.IncidentEquipments.Add(new IncidentEquipment
                {
                    Id = Guid.NewGuid(),
                    IncidentId = masterIncident.Id,
                    EquipmentId = brokenDetail.EquipmentId
                });

                // Link Detail -> Incident
                brokenDetail.Incident = masterIncident;
            }
        }

        // 5. GỌI REPO ĐỂ LƯU (Transaction nằm ở đây)
        await roomCheckRepository.AddRoomCheckTransactionAsync(
            roomCheck,
            masterIncident,
            updatedEquipments,
            cancellationToken);

        return roomCheck.Id;
    }
}