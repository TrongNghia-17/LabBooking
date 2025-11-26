using LabBooking.Application.Features.SecurityGuards.Dtos;

namespace LabBooking.Application.Features.SecurityGuards.Queries.GetByIdSecurityGuard
{
    public record GetSecurityGuardByIdQuery(Guid Id) : IRequest<SecurityGuardResponse>;
}
