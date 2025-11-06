using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.LabRooms.Commands.UpdateLabRoom;

/// <summary>
/// Command để xử lý logic cập nhật một LabRoom.
/// </summary>
public record UpdateLabRoomCommand() : IRequest<Unit>
{
    [JsonIgnore]
    public Guid Id { get; set; }

    public string LabName { get; set; } = default!;
    public string? Location { get; set; }
    public int? MaximumLimit { get; set; }
    public Guid? MainManagerId { get; set; }
    public bool IsActive { get; set; }
}
