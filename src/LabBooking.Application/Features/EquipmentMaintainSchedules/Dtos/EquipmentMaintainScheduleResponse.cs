namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

/// <summary>
/// DTO đại diện cho một lịch bảo trì thiết bị trả về cho client.
/// </summary>
public record EquipmentMaintainScheduleResponse
{
    public Guid Id { get; init; }
    public Guid EquipmentId { get; init; }
    public bool IsManyDay { get; init; }
    public bool? IsAllDay { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int? NumberOfSlot { get; init; }

    /// <summary>
    /// Trạng thái bảo trì (VD: "Done", "NotYet")
    /// (Sử dụng tên 'EquimentpMaintainStatus' giống trong entity)
    /// </summary>
    public string? EquimentpMaintainStatus { get; init; }
    public string? Description { get; init; }
}
