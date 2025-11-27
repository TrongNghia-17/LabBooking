namespace LabBooking.Application.Features.SecurityGuards.Commands.DeleteSecurityGuard
{
    public record DeleteSecurityGuardCommand(Guid Id) : IRequest<Unit>;
}
