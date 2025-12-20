namespace LabBooking.Application.Features.Dashboard.Dtos;

// DTO chung cho các mục trong biểu đồ (Tên, Số lượng)
public class StatItemDto
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

// DTO chính cho toàn bộ Dashboard
public class SystemHealthDashboardResponse
{
    public int UnresolvedIncidentsCount { get; set; }
    public int DevicesInMaintenanceCount { get; set; }
    public IEnumerable<StatItemDto> IncidentsByType { get; set; } = new List<StatItemDto>();
    public IEnumerable<StatItemDto> IncidentsByImportance { get; set; } = new List<StatItemDto>();
    public IEnumerable<StatItemDto> TopProblematicLabs { get; set; } = new List<StatItemDto>();
}
