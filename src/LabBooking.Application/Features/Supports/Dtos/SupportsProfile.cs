using LabBooking.Application.Features.Supports.Commands.CreateSupport;
using LabBooking.Application.Features.Supports.Commands.UpdateSupport;

namespace LabBooking.Application.Features.Supports.Dtos;

public class SupportsProfile : Profile
{
    public SupportsProfile()
    {
        CreateMap<Support, SupportsResponse>().ReverseMap();

        CreateMap<CreateSupportCommand, Support>()
            .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => string.Empty));

        CreateMap<UpdateSupportCommand, Support>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore());
    }
}
