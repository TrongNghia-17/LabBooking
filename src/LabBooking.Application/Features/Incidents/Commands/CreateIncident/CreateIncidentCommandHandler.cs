using LabBooking.Application.Services.Users;

namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

public class CreateIncidentCommandHandler(
    ILogger<CreateIncidentCommandHandler> logger,
    IMapper mapper,
    IIncidentRepository incidentRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<CreateIncidentCommand, Guid>
{
    public async Task<Guid> Handle(CreateIncidentCommand request, CancellationToken cancellationToken)
    {
        // 1. Lấy ID người báo cáo (User đang đăng nhập)
        var reporterId = currentUserService.UserId;

        if (reporterId == null)
        {
            logger.LogWarning("Cố gắng tạo Incident khi chưa đăng nhập.");
            throw new UnauthorizedAccessException("Bạn cần đăng nhập để báo cáo sự cố.");
        }

        logger.LogInformation("User {UserId} đang tạo báo cáo sự cố cho Lab {LabId}", reporterId, request.LabRoomId);

        // 2. Map dữ liệu
        var incident = mapper.Map<Incident>(request);

        // 3. Gán các field hệ thống
        incident.ReportedById = reporterId.Value;
        incident.CreatedAt = DateTime.UtcNow; // Bắt buộc dùng UtcNow cho Postgres
        incident.IsResolved = false; // Mặc định chưa giải quyết

        // 4. Lưu xuống DB
        var incidentId = await incidentRepository.Create(incident, cancellationToken);

        return incidentId;
    }
}
