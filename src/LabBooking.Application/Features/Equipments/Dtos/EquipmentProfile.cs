using LabBooking.Application.Features.Equipments.Commands.CreateEquipment;
using LabBooking.Application.Features.Equipments.Commands.UpdateEquipment;

namespace LabBooking.Application.Features.Equipments.Dtos;

/// <summary>
/// Defines AutoMapper profiles for Equipment entities and DTOs.
/// </summary>
public class EquipmentProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EquipmentProfile"/> class.
    /// </summary>
    public EquipmentProfile()
    {
        CreateMap<CreateEquipmentCommand, Equipment>();
        //.ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
        //    Enum.Parse<EquipmentStatus>(src.Status, true)
        //));

        CreateMap<UpdateEquipmentCommand, Equipment>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                Enum.Parse<EquipmentStatus>(src.Status, true)
            ));

        CreateMap<Equipment, EquipmentResponse>()
            .ForMember(dest => dest.EquipmentCategoryName,
                opt => opt.MapFrom(src => src.EquipmentCategory != null ? src.EquipmentCategory.Name : "Chưa phân loại"));
    }
}
