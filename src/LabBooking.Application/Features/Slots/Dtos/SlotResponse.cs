namespace LabBooking.Application.Features.Slots.Dtos;

public record SlotResponse(
    Guid Id,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int SlotIndex,
    string Label
);
