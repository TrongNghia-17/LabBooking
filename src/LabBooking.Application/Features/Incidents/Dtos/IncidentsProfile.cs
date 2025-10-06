namespace LabBooking.Application.Features.Incidents.Dtos;

public class IncidentsProfile : Profile
{
    public IncidentsProfile()
    {
        CreateMap<Incident, IncidentsResponse>().ReverseMap();

        CreateMap<CreateIncidentCommand, Incident>()
               .ForMember(d => d.Type,
                    opt => opt.MapFrom(s => Enum.Parse<IncidentType>(s.Type!, true)))
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.IsResolved, opt => opt.MapFrom(_ => false))
               .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
