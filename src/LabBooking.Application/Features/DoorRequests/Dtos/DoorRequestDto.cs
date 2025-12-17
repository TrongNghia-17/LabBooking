namespace LabBooking.Application.Features.DoorRequests.Dtos;

public class DoorRequestDto
{
    public Guid Id { get; set; }
    public string BookingCode { get; set; } = default!;
    public string Reason { get; set; } = default!;

    public string RequestedByName { get; set; } = default!;
    public string RequestedByPhoneNumber { get; set; } = default!;

    public string? RequestedByEmail { get; set; }
    public DateTime RequestTime { get; set; }
    public string Status { get; set; } = default!;
}
