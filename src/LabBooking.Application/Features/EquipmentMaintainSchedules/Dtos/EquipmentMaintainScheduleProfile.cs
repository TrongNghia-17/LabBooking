using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;
using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.UpdateEquipmentMaintainSchedule;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

public class EquipmentMaintainScheduleProfile : Profile
{
    public EquipmentMaintainScheduleProfile()
    {
        // Mapping cho Create
        CreateMap<CreateEquipmentMaintainScheduleCommand, EquipmentMaintainSchedule>();
        CreateMap<UpdateEquipmentMaintainScheduleCommand, EquipmentMaintainSchedule>();
        CreateMap<EquipmentMaintainSchedule, EquipmentMaintainScheduleResponse>()
            // Chuyển Enum sang string cho DTO
            .ForMember(dest => dest.EquimentpMaintainStatus, // Giữ nguyên lỗi chính tả
                       opt => opt.MapFrom(src => src.EquimentpMaintainStatus.ToString()));
    }
}