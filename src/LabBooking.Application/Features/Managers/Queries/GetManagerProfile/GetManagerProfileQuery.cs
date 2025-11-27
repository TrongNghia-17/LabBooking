using LabBooking.Application.Features.Managers.Dtos;

namespace LabBooking.Application.Features.Managers.Queries.GetManagerProfile;

public record GetManagerProfileQuery() : IRequest<ManagerProfileResponse>;
