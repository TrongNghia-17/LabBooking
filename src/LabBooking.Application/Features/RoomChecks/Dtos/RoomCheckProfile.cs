using LabBooking.Application.Features.RoomChecks.Commands.CreateRoomCheck;

namespace LabBooking.Application.Features.RoomChecks.Dtos;

public class RoomCheckProfile : Profile
{
    public RoomCheckProfile()
    {
        CreateMap<CreateRoomCheckCommand, RoomCheck>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CheckedAt, opt => opt.MapFrom(s => DateTime.UtcNow))
            .ForMember(dest => dest.GuardId, opt => opt.Ignore()); // Gán trong Handler
    }
}