namespace LabBooking.Application.Features.DoorRequests.Commands.CreateDoorRequest;

public record CreateDoorRequestCommand(
    string BookingCode,
    DateOnly RequestDate,    // [MỚI] Ngày cụ thể
    Guid SlotId,             // [MỚI] Slot cụ thể
    string Reason
) : IRequest<Guid>;
