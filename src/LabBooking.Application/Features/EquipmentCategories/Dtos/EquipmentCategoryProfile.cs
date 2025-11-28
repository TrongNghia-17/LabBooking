using LabBooking.Application.Features.Equipments.Dtos;

namespace LabBooking.Application.Features.EquipmentCategories.Dtos;

public class EquipmentCategoryProfile : Profile
{
    public EquipmentCategoryProfile()
    {
        CreateMap<EquipmentCategory, EquipmentCategoryResponse>()
            .ForMember(dest => dest.EquipmentCount,
                opt => opt.MapFrom(src => src.Equipments != null ? src.Equipments.Count : 0));

        CreateMap<Equipment, EquipmentSimpleResponse>()
            .ForMember(dest => dest.LabRoomName,
                opt => opt.MapFrom(src => src.LabRoom != null ? src.LabRoom.LabName : "Chưa gán phòng"))

            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => GetStatusVN(src.Status)));
    }

    private static string GetStatusVN(EquipmentStatus status)
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
