using LabBooking.Application.Features.RoomChecks.Commands.CreateRoomCheck;

namespace LabBooking.Application.Features.RoomChecks.Dtos;

public class RoomCheckProfile : Profile
{
    public RoomCheckProfile()
    {
        // 1. Map Command -> RoomCheck Entity (Header)
        CreateMap<CreateRoomCheckCommand, RoomCheck>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Tự sinh ID
            .ForMember(dest => dest.CheckedAt, opt => opt.MapFrom(s => DateTime.UtcNow))
            .ForMember(dest => dest.GuardId, opt => opt.Ignore()) // Sẽ gán trong Handler từ CurrentUser
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.EquipmentDetails));

        // 2. Map DTO con -> EquipmentCheckResult Entity (Detail)
        CreateMap<RoomCheckItemDto, EquipmentCheckResult>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IncidentId, opt => opt.Ignore()) // Sẽ gán logic sau
            .ForMember(dest => dest.Equipment, opt => opt.Ignore());
    }
}