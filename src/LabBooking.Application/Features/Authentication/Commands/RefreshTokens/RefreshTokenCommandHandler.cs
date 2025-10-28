namespace LabBooking.Application.Features.Authentication.Commands.RefreshTokens;

public class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IJwtService jwtService,
    UserManager<User> userManager,
    ILogger<RefreshTokenCommandHandler> logger,
    IRefreshTokenFactory refreshTokenFactory,
    IClaimsGenerator claimsGenerator
    ) : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // === 1. Xác thực Token cũ ===

        var oldRefreshTokenString = request.ExpiredRefreshToken;
        if (string.IsNullOrEmpty(oldRefreshTokenString))
            throw new UnauthorizedAccessException("Invalid refresh token.");

        var oldRefreshToken = await refreshTokenRepository.GetByTokenAsync(request.ExpiredRefreshToken, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        // PHÁT HIỆN TẤN CÔNG (REPLAY ATTACK):
        // Nếu token đã BỊ THU HỒI (Revoked != null) mà vẫn bị đem ra sử dụng
        // -> Đây là dấu hiệu token đã bị đánh cắp và đang được dùng lại.
        if (oldRefreshToken.IsRevoked)
        {
            logger.LogWarning(
                "Phát hiện tấn công Replay Attack: Refresh Token đã bị thu hồi đang được sử dụng. UserId: {UserId}, Token: {Token}",
                oldRefreshToken.UserId, oldRefreshToken.Token);

            // => HỦY HÀNG LOẠT: Thu hồi TẤT CẢ token còn hạn khác của user này
            await refreshTokenRepository.RevokeAllTokensByUserIdAsync(oldRefreshToken.UserId, cancellationToken);

            // Ném lỗi để buộc tất cả phiên (cả user thật và kẻ tấn công) phải đăng nhập lại
            throw new UnauthorizedAccessException("Token replay detected. All sessions have been logged out for security reasons.");
        }

        // Kiểm tra xem token có bị hết hạn không
        if (oldRefreshToken.IsExpired)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        // === 2. Lấy thông tin User ===

        var user = await userManager.FindByIdAsync(oldRefreshToken.UserId.ToString())
            ?? throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        // === 3. Tạo Access Token MỚI ===

        // Lấy thông tin claims MỚI NHẤT (ví dụ user vừa được cập nhật role)
        var claims = await claimsGenerator.GenerateClaimsAsync(user);
        var newAccessToken = jwtService.GenerateAccessToken(claims);

        // === 4. Tạo Refresh Token MỚI (Bảo mật xoay vòng) ===

        var newRefreshToken = refreshTokenFactory.Create(user.Id);

        // === 5. Thu hồi Token cũ, Lưu Token mới ===

        oldRefreshToken.Revoked = DateTime.UtcNow; // Thu hồi token CŨ
        refreshTokenRepository.Update(oldRefreshToken); // Cập nhật token CŨ
        await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        // Lưu cả 2 thay đổi (update + add) vào DB
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        // === 6. Trả về Response ===

        return new RefreshTokenResponse(
            newAccessToken,
            newRefreshToken.Token,
            newRefreshToken.Expires
        );
    }
}
