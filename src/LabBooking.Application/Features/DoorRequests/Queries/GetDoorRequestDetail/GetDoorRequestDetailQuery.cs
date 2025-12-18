using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetDoorRequestDetail;

public record GetDoorRequestDetailQuery(Guid Id) : IRequest<DoorRequestDetailDto>;

