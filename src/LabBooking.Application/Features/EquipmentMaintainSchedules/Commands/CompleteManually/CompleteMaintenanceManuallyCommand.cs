using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CompleteManually;

public class CompleteMaintenanceManuallyCommand : IRequest<Unit>
{
    // ID của lịch trình sẽ được lấy từ route của API
    [JsonIgnore]
    public Guid ScheduleId { get; set; }
}
