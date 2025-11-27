namespace LabBooking.Application.Features.RoomMaintainSchedules.Dtos;

/// <summary>
/// DTO đại diện cho một lịch bảo trì trả về cho client.
/// </summary>
public record RoomMaintainScheduleResponse
{
    public Guid Id { get; init; }
    public Guid LabRoomId { get; init; }
    public string LabRoomName { get; set; } = default!;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string RoomMaintainStatus { get; init; } = default!;
    public string Description { get; init; } = default!;
}
