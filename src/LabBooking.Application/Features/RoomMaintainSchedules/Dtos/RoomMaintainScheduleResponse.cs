namespace LabBooking.Application.Features.RoomMaintainSchedules.Dtos;

/// <summary>
/// DTO đại diện cho một lịch bảo trì trả về cho client.
/// </summary>
public record RoomMaintainScheduleResponse
{
    public Guid Id { get; init; }
    public Guid LabRoomId { get; init; }
    public bool IsManyDay { get; init; }
    public bool? IsAllDay { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int? NumberOfSlot { get; init; }

    /// <summary>
    /// Trạng thái bảo trì (VD: "Done", "NotYet")
    /// </summary>
    public string? RoomMaintainStatus { get; init; }
    public string? Description { get; init; }
}
