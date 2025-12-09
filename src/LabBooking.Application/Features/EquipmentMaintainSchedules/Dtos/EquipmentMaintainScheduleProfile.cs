using LabBooking.Application.Features.EquipmentMaintainSchedules.Commands.CreateEquipmentMaintainSchedule;

namespace LabBooking.Application.Features.EquipmentMaintainSchedules.Dtos;

public class EquipmentMaintainScheduleProfile : Profile
{
    public EquipmentMaintainScheduleProfile()
    {
        // 1. INPUT: Map Command -> Entity Cha
        CreateMap<CreateEquipmentMaintainScheduleCommand, EquipmentMaintainSchedule>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Details, opt => opt.Ignore());

        // 2. OUTPUT: Map Entity Cha -> Batch Response
        CreateMap<EquipmentMaintainSchedule, EquipmentMaintainBatchResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.TotalEquipments, opt => opt.MapFrom(src => src.Details.Count))
            .ForMember(dest => dest.Equipments, opt => opt.MapFrom(src => src.Details));

        // 3. OUTPUT: Map Entity Con -> DTO Con
        CreateMap<EquipmentMaintenance, MaintainedEquipmentDto>()
            .ForMember(dest => dest.MaintenanceId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment != null ? src.Equipment.EquipmentName : "Unknown"))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<EquipmentMaintainSchedule, EquipmentMaintainScheduleResponse>()
                // Convert Enum Status sang String
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))

                // Đếm số lượng thiết bị
                .ForMember(dest => dest.EquipmentCount, opt => opt.MapFrom(src => src.Details.Count))

                // Lấy danh sách tên các thiết bị để hiển thị nhanh (VD: "Máy hàn, Máy cắt...")
                .ForMember(dest => dest.EquipmentNames, opt => opt.MapFrom(src =>
                    src.Details.Select(d => d.Equipment.EquipmentName).ToList()));
    }
}
