namespace LabBooking.Application.Features.Incidents.Dtos;

public class IncidentResponse
{
    public Guid Id { get; set; }
    public string LabRoomName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ImportanceLevel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? EquipmentName { get; set; } // Null nếu không hỏng máy
    public List<EquipmentSimpleResponse> Equipments { get; set; }

    // --- THÔNG TIN NHẠY CẢM (Sẽ ẩn nếu là Guard) ---
    public string? ReportedByName { get; set; }
    public string? ReportedByPhone { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
