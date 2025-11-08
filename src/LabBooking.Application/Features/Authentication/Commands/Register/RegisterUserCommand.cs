namespace LabBooking.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Command containing the information to register a new user.
/// </summary>
public record RegisterUserCommand(
    string Email,
    string Password,
    string ConfirmPassword
) : IRequest<Unit>;
