namespace LabBooking.Application.Features.Incidents.Queries.GetAllIncidents;

public class GetIncidentsQuery : IRequest<IEnumerable<IncidentResponse>>
{
    // 1. Lọc theo thời gian (Từ ngày... Đến ngày...)
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    // 2. Lọc theo trạng thái (Đã xong / Chưa xong)
    public bool? IsResolved { get; set; }

    // 3. Lọc theo mức độ (Low/Medium/High)
    public LevelOfImportance? Importance { get; set; }

    // 4. Lọc theo Phòng Lab (Dành cho Bảo vệ chọn phòng cụ thể)
    // Manager cũng dùng được, nhưng chỉ chọn được phòng mình quản lý.
    public Guid? LabRoomId { get; set; }

    // (Optional) Sắp xếp
    public bool IsDescending { get; set; } = true;
}
