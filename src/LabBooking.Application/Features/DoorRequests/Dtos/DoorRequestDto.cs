namespace LabBooking.Application.Features.DoorRequests.Dtos;

public record DoorRequestDto
{
    public Guid Id { get; init; }
    public string BookingCode { get; init; } = string.Empty;
    public DateOnly RequestDate { get; init; }  // [MỚI]
    public Guid SlotId { get; init; }            // [MỚI]
    public string Reason { get; init; } = string.Empty;
    public string? ManagerNote { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime RequestTime { get; init; }
    public DateTime? AcceptedTime { get; init; }
    public string ContactName { get; set; } = string.Empty;
    public string RequestedByEmail { get; set; } = string.Empty;
    public string RequestedByPhoneNumber { get; set; } = string.Empty;
}
