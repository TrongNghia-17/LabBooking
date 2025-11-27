namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

public record EquipmentMaintainScheduleResponse
{
    public Guid Id { get; init; }
    public Guid EquipmentId { get; init; }
    public string? EquipmentName { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string EquimentpMaintainStatus { get; init; } = default!;
    public string Description { get; init; } = default!;
}
