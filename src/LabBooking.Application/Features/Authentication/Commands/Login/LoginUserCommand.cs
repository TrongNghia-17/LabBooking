using LabBooking.Application.Features.Authentication.Dtos;

namespace LabBooking.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Represents the command with data needed to log in a user.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
public record LoginUserCommand(
    string Email,
    string Password
) : IRequest<AuthResponse>;
