namespace LabBooking.Application.Features.Booking.Dtos;

public record BookingLookupDto
{
    public Guid Id { get; init; }
    public string BookingCode { get; init; } = string.Empty;
    public string LabName { get; init; } = string.Empty;
    public DateOnly Date { get; init; }
    public string TimeSlot { get; init; } = string.Empty;


    public string? RequesterFullName { get; init; }
    public string? RequesterEmail { get; init; }
    public string? RequesterPhoneNumber { get; init; }
}
