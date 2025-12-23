using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Features.Dashboard.Dtos;

namespace LabBooking.Application.Features.Dashboard.Queries.GetSystemHealthDashboardQuery;

public class GetSystemHealthDashboardQueryHandler(
    IIncidentRepository incidentRepo,
    IEquipmentRepository equipmentRepo, // Thêm repo này
    ICurrentUserService currentUserService,
    IMapper mapper
) : IRequestHandler<GetSystemHealthDashboardQuery, SystemHealthDashboardResponse>
{
    public async Task<SystemHealthDashboardResponse> Handle(GetSystemHealthDashboardQuery request, CancellationToken cancellationToken)
    {
        // Phân quyền: Admin xem toàn bộ, Manager chỉ xem của mình
        Guid? managerId = null;
        if (!currentUserService.Roles.Contains(Roles.Admin))
        {
            managerId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        }

        var unresolvedIncidents = await incidentRepo.GetUnresolvedCountAsync(managerId, cancellationToken);
        var maintenanceDevices = await equipmentRepo.GetMaintenanceCountAsync(managerId, cancellationToken);
        var incidentsByType = await incidentRepo.GetStatsByTypeAsync(managerId, 30, cancellationToken);
        var incidentsByImportance = await incidentRepo.GetStatsByImportanceAsync(managerId, 30, cancellationToken);
        var topLabs = await incidentRepo.GetTopProblematicLabsAsync(managerId, 90, 5, cancellationToken);

        // Tập hợp kết quả và map sang DTO
        var response = new SystemHealthDashboardResponse
        {
            UnresolvedIncidentsCount = unresolvedIncidents,
            DevicesInMaintenanceCount = maintenanceDevices,
            IncidentsByType = mapper.Map<IEnumerable<StatItemDto>>(incidentsByType),
            IncidentsByImportance = mapper.Map<IEnumerable<StatItemDto>>(incidentsByImportance),
            TopProblematicLabs = mapper.Map<IEnumerable<StatItemDto>>(topLabs)
        };

        return response;
    }
}