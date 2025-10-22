using LabBooking.Application.Features.Auths.Dtos;

namespace LabBooking.Application.Features.Auths.Commands;

public record GoogleLoginCommand(string IdToken) : IRequest<AuthResponse>;
