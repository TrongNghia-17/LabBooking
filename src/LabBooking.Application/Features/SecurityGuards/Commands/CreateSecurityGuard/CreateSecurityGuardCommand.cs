namespace LabBooking.Application.Features.SecurityGuards.Commands.Register;

/// <summary>
/// Command containing the information to register a new user.
/// </summary>
public record CreateSecurityGuardCommand(
    string Email,
    string Password,
    string ConfirmPassword
) : IRequest<Unit>;
