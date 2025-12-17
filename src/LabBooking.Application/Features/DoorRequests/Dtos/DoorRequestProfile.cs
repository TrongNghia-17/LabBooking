using LabBooking.Application.Features.DoorRequests.Commands.CreateDoorRequest;

namespace LabBooking.Application.Features.DoorRequests.Dtos;

public class DoorRequestProfile : Profile
{
    public DoorRequestProfile()
    {
        CreateMap<CreateDoorRequestCommand, DoorOpeningRequest>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.RequestTime, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DoorRequestStatus.Pending))
            .ForMember(dest => dest.ManagerId, opt => opt.Ignore())
            .ForMember(dest => dest.RequestedById, opt => opt.Ignore());

        CreateMap<DoorOpeningRequest, DoorRequestDto>()
            .ForMember(dest => dest.RequestedByEmail, opt => opt.MapFrom(src => src.RequestedBy!.Email))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.RequestedByName, opt => opt.MapFrom(src => src.RequestedBy!.FullName ?? "Unknown"))
            .ForMember(dest => dest.RequestedByPhoneNumber, opt => opt.MapFrom(src => src.RequestedBy!.PhoneNumber ?? "N/A"));
    }
}
