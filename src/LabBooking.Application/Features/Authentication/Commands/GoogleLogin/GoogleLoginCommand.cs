using LabBooking.Application.Features.Authentication.Dtos;

namespace LabBooking.Application.Features.Authentication.Commands.GoogleLogin;

/// <summary>
/// Command to log in a user via a Google ID Token.
/// </summary>
/// <param name="IdToken">The ID Token provided by Google.</param>
public record GoogleLoginCommand(string IdToken) : IRequest<AuthResponse>;
