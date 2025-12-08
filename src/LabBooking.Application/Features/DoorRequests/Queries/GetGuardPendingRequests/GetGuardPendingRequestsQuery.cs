using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetGuardPendingRequests;

public record GetGuardPendingRequestsQuery : IRequest<IEnumerable<GuardRequestDto>>;
