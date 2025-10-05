namespace LabBooking.Application.Features.Incidents.Dtos;

public class IncidentsProfile : Profile
{
    public IncidentsProfile()
    {
        CreateMap<Incident, GetAllIncidentsResponse>().ReverseMap();
    }
}
