namespace LabBooking.Application.Features.Booking.Dtos;

public record BookingLookupDto
{
    public Guid Id { get; init; }
    public string BookingCode { get; init; } = string.Empty;
    public string LabName { get; init; } = string.Empty;

    // Thay vì 1 ngày và 1 slot, giờ là danh sách các ngày kèm slots
    public List<BookingDateSlotDto> DateSlots { get; init; } = new();

    // Thông tin người đặt (chỉ hiện với Manager)
    public string? RequesterFullName { get; init; }
    public string? RequesterEmail { get; init; }
    public string? RequesterPhoneNumber { get; init; }
}

public record BookingDateSlotDto
{
    public DateOnly Date { get; init; }
    public List<SlotInfoDto> Slots { get; init; } = new();
}

public record SlotInfoDto
{
    public Guid SlotId { get; init; }
    public string SlotLabel { get; init; } = string.Empty; // VD: "Ca 1", "Ca 2"
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
}