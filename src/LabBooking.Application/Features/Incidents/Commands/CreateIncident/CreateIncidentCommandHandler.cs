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
        // 1. Check Login
        var reporterId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Bạn cần đăng nhập để báo cáo sự cố.");

        // 2. Check Phòng Lab tồn tại
        var labExists = await labRoomRepository.ExistsAsync(request.LabRoomId, cancellationToken);
        if (!labExists)
            throw new NotFoundException(nameof(LabRoom), request.LabRoomId.ToString());

        // 3. (MỚI) CHECK LOGIC TRÙNG LẶP / SPAM
        // Logic: Nếu User này vừa báo cáo loại lỗi này ở phòng này cách đây < 1 phút -> Chặn
        bool isSpam = await incidentRepository.IsSpamAsync(reporterId, request.LabRoomId, request.Type, cancellationToken);
        if (isSpam)
            throw new BadRequestException("Bạn vừa báo cáo sự cố này rồi. Vui lòng đợi một lát nếu muốn báo cáo tiếp.");

        if (request.Type == IncidentType.EquipmentFailure && request.EquipmentId.HasValue)
        {
            // Lấy thiết bị ra
            var equipment = await equipmentRepository.GetByIdAsync(request.EquipmentId.Value);

            // Validate kỹ: Máy này có thuộc phòng Lab đang báo cáo không?
            if (equipment == null || equipment.LabRoomId != request.LabRoomId)
            {
                throw new BadRequestException("Thiết bị không thuộc phòng Lab này.");
            }

            // Đổi trạng thái sang Hỏng (Broken)
            if (equipment.Status != EquipmentStatus.Broken)
            {
                equipment.Status = EquipmentStatus.Broken;
                equipment.IsAvailable = false;

                // Cần hàm Update trong EquipmentRepo
                await equipmentRepository.UpdateAsync(equipment, cancellationToken);
            }
        }

        // 4. MAP DỮ LIỆU (Dùng AutoMapper)
        var incident = mapper.Map<Incident>(request);

        // 5. GÁN CÁC GIÁ TRỊ CÒN THIẾU (Decorate)
        incident.Id = Guid.NewGuid();
        incident.ReportedById = reporterId;
        incident.CreatedAt = DateTime.UtcNow; // Luôn dùng UTC
        incident.IsResolved = false;
        incident.SlotId = null;
        incident.EquipmentId = request.EquipmentId;

        // 6. Lưu vào DB
        await incidentRepository.CreateAsync(incident, cancellationToken);

        logger.LogInformation("Incident Created: User {User} báo cáo {Type} tại phòng {Room}.", reporterId, request.Type, request.LabRoomId);

        return incident.Id;
    }
}
