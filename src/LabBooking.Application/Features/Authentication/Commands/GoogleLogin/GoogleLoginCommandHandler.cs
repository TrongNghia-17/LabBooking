using LabBooking.Application.Features.Authentication.Dtos;

namespace LabBooking.Application.Features.Authentication.Commands.GoogleLogin;

/// <summary>
/// Handles the GoogleLoginCommand to authenticate or register a user
/// based on their Google ID Token.
/// </summary>
public class GoogleLoginCommandHandler(
    IGoogleAuthService googleAuthService,
    IJwtService jwtService,
    UserManager<User> userManager,
    IUserFactory userFactory,
    IRefreshTokenRepository refreshTokenRepository,
    IRefreshTokenFactory refreshTokenFactory,
    IClaimsGenerator claimsGenerator
    ) : IRequestHandler<GoogleLoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            throw new BadRequestException("Missing idToken.");

        // 1. Validate the Google token
        var googlePayload = await googleAuthService.VerifyGoogleTokenAsync(request.IdToken)
            ?? throw new UnauthorizedAccessException("Invalid Google token");

        // 2. Check the email domain
        string userRole;
        if (googlePayload.Email.EndsWith("@fpt.edu.vn", StringComparison.OrdinalIgnoreCase))
            userRole = "User";
        else
            throw new ForbidException("Email domain is not allowed.");

        // 3. Check the campus
        var campusInfo = googlePayload.FamilyName;
        if (string.IsNullOrWhiteSpace(campusInfo) || !campusInfo.Contains("HCM", StringComparison.OrdinalIgnoreCase))
            throw new ForbidException("Account does not belong to HCM facility.");

        // 4. Find the user in the database
        var user = await userManager.FindByEmailAsync(googlePayload.Email);

        if (user == null)
        {
            // 5. If user doesn't exist -> Create a new user
            user = userFactory.CreateUserFromGooglePayload(googlePayload);

            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(user, userRole);
        }

        // 6. Get roles and generate claims
        var claims = await claimsGenerator.GenerateClaimsAsync(user);

        // 7. Generate a new Access Token
        var jwt = jwtService.GenerateAccessToken(claims);

        // 8. Create and save the Refresh Token
        var newRefreshToken = refreshTokenFactory.Create(user.Id);

        await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        // 9. Create the final authentication response
        var authResponse = new AuthResponse
        (
            jwt,
            newRefreshToken.Token,
            newRefreshToken.Expires
        );

        return authResponse;
    }
}
