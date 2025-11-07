using LabBooking.Application.Features.Equipments.Commands.CreateEquipment;
using LabBooking.Application.Features.Equipments.Commands.UpdateEquipment;

namespace LabBooking.Application.Features.Equipments.Dtos;

public class EquipmentProfile : Profile
{
    public EquipmentProfile()
    {
        CreateMap<CreateEquipmentCommand, Equipment>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                Enum.Parse<EquipmentStatus>(src.Status, true)
            ));

        CreateMap<UpdateEquipmentCommand, Equipment>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                Enum.Parse<EquipmentStatus>(src.Status, true)
            ));

        CreateMap<Equipment, EquipmentResponse>();
    }
}
