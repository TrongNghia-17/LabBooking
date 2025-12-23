using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetDoorRequestQr;

public record GetDoorRequestQrQuery(Guid Id) : IRequest<DoorRequestQrDto>;

