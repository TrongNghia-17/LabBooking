using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Commands.UpdateStatus
{
    public record UpdateRoomMaintainStatusCommand : IRequest<Unit>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public RoomMaintainStatus Status { get; set; }
    }
}
