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

        CreateMap<RoomCheck, RoomCheckDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString())) // Enum -> String
            .ForMember(dest => dest.LabRoomName, opt => opt.MapFrom(src =>
                src.LabRoom != null ? src.LabRoom.LabName : "Unknown"))
            .ForMember(dest => dest.GuardName, opt => opt.MapFrom(src =>
                src.Guard != null ? src.Guard.FullName : "Unknown"))
            // Map SlotName (ví dụ: Ca 1 (7h-9h))
            .ForMember(dest => dest.SlotName, opt => opt.MapFrom(src =>
                src.Slot != null ? src.Slot.Label : "Không xác định"));
    }
}