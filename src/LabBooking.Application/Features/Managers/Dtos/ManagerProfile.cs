namespace LabBooking.Application.Features.Managers.Dtos;

public class ManagerProfile : Profile
{
    public ManagerProfile()
    {
        CreateMap<Equipment, ManagerEquipmentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetEquipmentStatusVN(src.Status)));

        CreateMap<LabRoom, ManagerLabRoomDto>()
            .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.LabName ?? "Chưa đặt tên"))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? "Đang hoạt động" : "Ngừng hoạt động"))
            .ForMember(dest => dest.EquipmentGroups, opt => opt.Ignore());
    }

    private static string GetEquipmentStatusVN(EquipmentStatus status)
    {
        return status switch
        {
            EquipmentStatus.Available => "Sẵn sàng",
            EquipmentStatus.Maintain => "Đang bảo trì",
            EquipmentStatus.Broken => "Hỏng",
            EquipmentStatus.Other => "Khác",
            _ => "Không xác định"
        };
    }
}
