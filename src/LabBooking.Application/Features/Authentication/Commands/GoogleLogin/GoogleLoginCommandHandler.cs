using LabBooking.Application.Features.Authentication.Dtos;

namespace LabBooking.Application.Features.Authentication.Commands.GoogleLogin;

public class GoogleLoginCommandHandler(
    IGoogleAuthService googleAuthService,
    IJwtService jwtService,
    UserManager<User> userManager,
    IMapper mapper,
    IUserFactory userFactory,
    IRefreshTokenRepository refreshTokenRepository,
    IRefreshTokenFactory refreshTokenFactory,
    IClaimsGenerator claimsGenerator
    ) : IRequestHandler<GoogleLoginCommand, GoogleLoginResponse>
{
    public async Task<GoogleLoginResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            throw new BadRequestException("Missing idToken.");

        // 1. Xác thực Google
        var googlePayload = await googleAuthService.VerifyGoogleTokenAsync(request.IdToken)
            ?? throw new UnauthorizedAccessException("Invalid Google token");

        // 2. Kiểm tra domain
        string userRole;
        if (googlePayload.Email.EndsWith("@fpt.edu.vn", StringComparison.OrdinalIgnoreCase))
            userRole = "Student";
        else if (googlePayload.Email.EndsWith("@fe.edu.vn", StringComparison.OrdinalIgnoreCase))
            userRole = "Teacher";
        else
            throw new ForbidException("Email domain is not allowed.");

        // 3. Kiểm tra cơ sở (Campus)
        var campusInfo = googlePayload.FamilyName;
        if (string.IsNullOrWhiteSpace(campusInfo) || !campusInfo.Contains("HCM", StringComparison.OrdinalIgnoreCase))
            throw new ForbidException("Account does not belong to HCM facility.");

        // 4. Tìm user trong DB
        var user = await userManager.FindByEmailAsync(googlePayload.Email);

        if (user == null)
        {
            // 5. Nếu không có -> Tạo User mới
            user = userFactory.CreateUserFromGooglePayload(googlePayload);

            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(user, userRole);
        }

        // 6. Lấy roles và tạo claims
        var claims = await claimsGenerator.GenerateClaimsAsync(user);

        // 7. Gọi phương thức GenerateAccessToken mới
        var jwt = jwtService.GenerateAccessToken(claims);
        var userDto = mapper.Map<UserDto>(user);

        // 8. Tạo và Lưu Refresh Token
        var newRefreshToken = refreshTokenFactory.Create(user.Id);

        await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new GoogleLoginResponse
        (
            jwt,
            userDto,
            newRefreshToken.Token,
            newRefreshToken.Expires
        );
    }
}
