using LabBooking.Application.Features.RoomMaintainSchedules.Commands.CreateRoomMaintainSchedule;
using LabBooking.Application.Features.RoomMaintainSchedules.Commands.UpdateRoomMaintainSchedule;

namespace LabBooking.Application.Features.RoomMaintainSchedules.Dtos;

public class RoomMaintainScheduleProfile : Profile
{
    public RoomMaintainScheduleProfile()
    {
        // Mapping cho Create
        CreateMap<CreateRoomMaintainScheduleCommand, RoomMaintainSchedule>();
        CreateMap<UpdateRoomMaintainScheduleCommand, RoomMaintainSchedule>();

        CreateMap<RoomMaintainSchedule, RoomMaintainScheduleResponse>()
            // Chuyển Enum sang string cho DTO
            .ForMember(dest => dest.RoomMaintainStatus,
                       opt => opt.MapFrom(src => src.RoomMaintainStatus.ToString()));
    }
}
