using LabBooking.Application.Features.Authentication.Dtos;

namespace LabBooking.Application.Features.Authentication.Commands.GoogleLogin;

public record GoogleLoginResponse(
    string AccessToken,
    UserDto UserDto,
    string RefreshToken,
    DateTime RefreshTokenExpiry);
