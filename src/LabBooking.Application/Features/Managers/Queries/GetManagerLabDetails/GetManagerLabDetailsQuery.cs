using LabBooking.Application.Features.Managers.Dtos;

namespace LabBooking.Application.Features.Managers.Queries.GetManagerLabDetails;

public record GetManagerLabDetailsQuery() : IRequest<ManagerLabDetailsResponse>;
