using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetOpenableBooking;

public record GetOpenableBookingsQuery(Guid UserId) : IRequest<List<BookingForDoorOpenDto>>;

