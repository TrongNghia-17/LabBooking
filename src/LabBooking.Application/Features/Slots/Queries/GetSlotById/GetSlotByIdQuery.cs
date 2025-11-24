using LabBooking.Application.Features.Slots.Dtos;

namespace LabBooking.Application.Features.Slots.Queries.GetSlotById;

public record GetSlotByIdQuery(Guid Id) : IRequest<SlotResponse>;
