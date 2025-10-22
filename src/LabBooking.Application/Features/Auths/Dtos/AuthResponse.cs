namespace LabBooking.Application.Features.Auths.Dtos;

public class AuthResponse
{
    public string AccessToken { get; set; } = default!;
    public UserDto UserDto { get; set; } = default!;
}
