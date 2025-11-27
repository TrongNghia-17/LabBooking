namespace LabBooking.Application.Features.SecurityGuards.Dtos
{
    public class SecurityGuardProfile : Profile
    {
        public SecurityGuardProfile()
        {
            CreateMap<User, SecurityGuardResponse>();
        }
    }
}
