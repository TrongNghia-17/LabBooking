namespace LabBooking.Application.Features.Auths.Commands;

public class GoogleLoginCommandHandler(
    IGoogleAuthService googleAuthService,
    IJwtService jwtService,
    UserManager<User> userManager,
    IMapper mapper,
    IUserFactory userFactory
    ) : IRequestHandler<GoogleLoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            throw new BadRequestException("Missing idToken.");

        // 1. Xác thực Google
        var googlePayload = await googleAuthService.VerifyGoogleTokenAsync(request.IdToken)
            ?? throw new UnauthorizedAccessException("Invalid Google token");

        // 2. Kiểm tra domain
        if (!googlePayload.Email.EndsWith("@fpt.edu.vn", StringComparison.OrdinalIgnoreCase))
            throw new ForbidException("Email domain is not allowed.");

        // 3. Kiểm tra cơ sở (Campus)
        var campusInfo = googlePayload.FamilyName;
        if (string.IsNullOrWhiteSpace(campusInfo) || !campusInfo.Contains("HCM", StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbidException("Tài khoản không thuộc cơ sở HCM.");
        }

        // 4. Tìm user trong DB
        var user = await userManager.FindByEmailAsync(googlePayload.Email);

        if (user == null)
        {
            // 5. Nếu không có -> Tạo User mới
            user = userFactory.CreateUserFromGooglePayload(googlePayload);

            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));
        }

        var jwt = jwtService.GenerateToken(user.Email!, user.UserName!);

        var userDto = mapper.Map<UserDto>(user);

        return new AuthResponse
        {
            AccessToken = jwt,
            UserDto = userDto
        };
    }
}
