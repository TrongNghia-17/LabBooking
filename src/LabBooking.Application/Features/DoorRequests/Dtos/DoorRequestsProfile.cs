namespace LabBooking.Application.Features.DoorRequests.Dtos;

public class DoorRequestsProfile : Profile
{
    public DoorRequestsProfile()
    {
        CreateMap<DoorRequest, DoorRequestsResponse>().ReverseMap();
    }
}
