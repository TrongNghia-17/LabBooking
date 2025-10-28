namespace LabBooking.Application.Features.Authentication.Commands.GoogleLogin;

public record GoogleLoginCommand(string IdToken) : IRequest<GoogleLoginResponse>;
