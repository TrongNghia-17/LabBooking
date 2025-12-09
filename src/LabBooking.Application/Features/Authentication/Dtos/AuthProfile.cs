using LabBooking.Application.Features.SecurityGuards.Commands.Register;

namespace LabBooking.Application.Features.Authentication.Dtos;

/// <summary>
/// AutoMapper profile for authentication-related DTOs.
/// </summary>
public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, UserProfileResponse>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => new List<string>()))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));

        CreateMap<CreateSecurityGuardCommand, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true));
    }
}
