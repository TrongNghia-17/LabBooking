namespace LabBooking.Application.Features.Authentication.Dtos;

/// <summary>
/// AutoMapper profile for authentication-related DTOs.
/// </summary>
public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, UserProfileResponse>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => new List<string>()));
    }
}
