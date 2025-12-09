using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.UpdateRoomMaintainSchedule;

/// <summary>
/// Command chứa dữ liệu để cập nhật một lịch bảo trì.
/// </summary>
public record UpdateRoomMaintainScheduleCommand() : IRequest<Unit>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Description { get; set; }
}
