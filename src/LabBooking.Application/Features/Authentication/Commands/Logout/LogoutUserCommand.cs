namespace LabBooking.Application.Features.Authentication.Commands.Logout;

/// <summary>
/// Command to log out the currently authenticated user.
/// </summary>
public record LogoutUserCommand() : IRequest<Unit>;
