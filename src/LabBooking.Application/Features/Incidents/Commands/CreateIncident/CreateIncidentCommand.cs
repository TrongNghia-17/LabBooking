namespace LabBooking.Application.Features.Incidents.Commands.CreateIncident;

/// <summary>
/// Record chứa dữ liệu đầu vào để tạo một sự cố mới.
/// </summary>
/// <param name="LabRoomId">ID của phòng lab nơi sự cố xảy ra.</param>
/// <param name="ReportedById">ID của người dùng báo cáo sự cố.</param>
/// <param name="Type">Loại sự cố (ví dụ: Fire, PowerOutage).</param>
/// <param name="Description">Mô tả chi tiết về sự cố.</param>
public record CreateIncidentCommand(
    Guid LabRoomId,
    Guid ReportedById,
    string Type,
    string Description
) : IRequest<IncidentsResponse>;
