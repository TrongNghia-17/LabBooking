using LabBooking.Application.Features.Authentication.Dtos;
using LabBooking.Application.Interfaces.Authentication.Token;

namespace LabBooking.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Handles the LoginCommand to authenticate a user.
/// </summary>
public class LoginUserCommandHandler(
    UserManager<User> userManager,
    IJwtService jwtService,
    IClaimsGenerator claimsGenerator,
    IRefreshTokenFactory refreshTokenFactory,
    IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<LoginUserCommand, AuthResponse>
{
    /// <summary>
    /// Handles the user login request.
    /// </summary>
    /// <param name="request">The login command containing email and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An AuthResponse containing new access and refresh tokens.</returns>
    /// <exception cref="AuthenticationException">Thrown if email or password is invalid.</exception>
    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Find the user by email
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        // 2. Validate the password
        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // 3. Generate claims for the user
        var claims = await claimsGenerator.GenerateClaimsAsync(user);

        // 4. Generate a new Access Token
        var accessToken = jwtService.GenerateAccessToken(claims);

        // 5. Generate and save a new Refresh Token
        var refreshToken = refreshTokenFactory.Create(user.Id);
        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        // 6. Create the authentication response 
        var authResponse = new AuthResponse(
            accessToken,
            refreshToken.Token,
            refreshToken.Expires
        );

        return authResponse;
    }
}
