using LabBooking.Application.Features.DoorRequests.Commands.CreateDoorRequest;

namespace LabBooking.Application.Features.DoorRequests.Dtos;

public class DoorRequestProfile : Profile
{
    public DoorRequestProfile()
    {
        CreateMap<CreateDoorRequestCommand, DoorOpeningRequest>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Tự sinh ID
            .ForMember(dest => dest.RequestTime, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DoorRequestStatus.Pending))
            .ForMember(dest => dest.ManagerId, opt => opt.Ignore()) // Sẽ map thủ công trong Handler
            .ForMember(dest => dest.RequestedById, opt => opt.Ignore()); // Sẽ map thủ công từ CurrentUser
    }
}
