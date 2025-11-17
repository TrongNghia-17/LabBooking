using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.UpdateEquipmentMaintainSchedule;

/// <summary>
/// Command chứa dữ liệu để cập nhật một lịch bảo trì thiết bị.
/// </summary>
public record UpdateEquipmentMaintainScheduleCommand() : IRequest<Unit>
{
    /// <summary>
    /// ID của lịch bảo trì, lấy từ route
    /// </summary>
    [JsonIgnore]
    public Guid Id { get; set; }

    // Các trường có thể cập nhật, lấy từ entity
    public Guid EquipmentId { get; set; }
    public bool IsManyDay { get; set; }
    public bool? IsAllDay { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? NumberOfSlot { get; set; }

    /// <summary>
    /// Cho phép cập nhật trạng thái, ví dụ: từ "NotYet" sang "Done"
    /// (Tên enum giữ nguyên lỗi chính tả như trong entity)
    /// </summary>
    public EquimentpMaintainStatus? EquimentpMaintainStatus { get; set; }
    public string? Description { get; set; }
}
