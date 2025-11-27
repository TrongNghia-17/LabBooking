using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.UpdateEquipmentMaintainSchedule;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

public class EquipmentMaintainScheduleProfile : Profile
{
    public EquipmentMaintainScheduleProfile()
    {
        CreateMap<CreateEquipmentMaintainScheduleCommand, EquipmentMaintainSchedule>();
        CreateMap<UpdateEquipmentMaintainScheduleCommand, EquipmentMaintainSchedule>();
        CreateMap<EquipmentMaintainSchedule, EquipmentMaintainScheduleResponse>()
            .ForMember(dest => dest.EquimentpMaintainStatus,
                       opt => opt.MapFrom(src => src.EquimentpMaintainStatus.ToString()));
    }
}