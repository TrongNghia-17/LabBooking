using LabBooking.Application.Features.Authentication.Dtos;
using LabBooking.Application.Interfaces.Authentication.Token;
using System.Security.Authentication;

namespace LabBooking.Application.Features.Authentication.Commands.RefreshTokens;

/// <summary>
/// Handles the RefreshTokenCommand to issue new tokens.
/// Implements token rotation and replay attack detection.
/// </summary>
public class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IJwtService jwtService,
    UserManager<User> userManager,
    ILogger<RefreshTokenCommandHandler> logger,
    IRefreshTokenFactory refreshTokenFactory,
    IClaimsGenerator claimsGenerator
    ) : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // === 1. Validate Old Token ===

        var oldRefreshTokenString = request.ExpiredRefreshToken;
        if (string.IsNullOrEmpty(oldRefreshTokenString))
            throw new AuthenticationException("Invalid refresh token.");

        var oldRefreshToken = await refreshTokenRepository.GetByTokenAsync(request.ExpiredRefreshToken, cancellationToken)
            ?? throw new AuthenticationException("Invalid or expired refresh token.");

        // REPLAY ATTACK DETECTION:
        // If a token is already revoked but is used again,
        // it's a sign of a stolen token being re-used.
        if (oldRefreshToken.IsRevoked)
        {
            logger.LogWarning(
                "Replay Attack Detected: Revoked refresh token was used. UserId: {UserId}, Token: {Token}",
                oldRefreshToken.UserId, oldRefreshToken.Token);

            // => Revoke ALL other valid tokens of this user
            await refreshTokenRepository.RevokeAllTokensByUserIdAsync(oldRefreshToken.UserId, cancellationToken);

            throw new AuthenticationException("All sessions have been logged out for security reasons.");
        }

        // Check if the token is expired
        if (oldRefreshToken.IsExpired)
            throw new AuthenticationException("Invalid or expired refresh token.");

        // === 2. Get User Information ===

        var user = await userManager.FindByIdAsync(oldRefreshToken.UserId.ToString())
            ?? throw new AuthenticationException("User not found for the provided token.");

        // === 3. Generate NEW Access Token ===

        // Get the LATEST claims (e.g., if user roles were updated)
        var claims = await claimsGenerator.GenerateClaimsAsync(user);
        var newAccessToken = jwtService.GenerateAccessToken(claims);

        // === 4. Generate NEW Refresh Token (Token Rotation) ===

        var newRefreshToken = refreshTokenFactory.Create(user.Id);

        // === 5. Revoke Old Token, Save New Token ===

        oldRefreshToken.Revoked = DateTime.UtcNow;
        refreshTokenRepository.Update(oldRefreshToken);

        await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        // Save both changes (update + add) to the DB
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        // === 6. Return Response ===

        var authResponse = new AuthResponse(
            newAccessToken,
            newRefreshToken.Token,
            newRefreshToken.Expires
        );

        return authResponse;
    }
}
