namespace LabBooking.Application.Features.Incidents.Dtos;

public class IncidentsProfile : Profile
{
    public IncidentsProfile()
    {
        CreateMap<Incident, IncidentsResponse>().ReverseMap();

        CreateMap<CreateIncidentCommand, Incident>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ReportedById, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsResolved, opt => opt.Ignore())
            .ForMember(dest => dest.SlotId, opt => opt.Ignore());
    }
}
