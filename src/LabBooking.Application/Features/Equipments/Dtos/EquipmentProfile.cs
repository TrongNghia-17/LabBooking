using LabBooking.Application.Features.Equipments.Commands.CreateEquipment;

namespace LabBooking.Application.Features.Equipments.Dtos;

public class EquipmentProfile : Profile
{
    public EquipmentProfile()
    {
        // Mapping cho Create:
        CreateMap<CreateEquipmentCommand, Equipment>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.Status)
                    ? EquipmentStatus.Available
                    : Enum.Parse<EquipmentStatus>(src.Status, true)
            ))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src =>
                src.IsAvailable ?? true));

        // Mapping cho Response DTO (tương tự LabRoom -> LabRoomResponse)
        CreateMap<Equipment, EquipmentResponse>();

        // TODO: Bạn có thể thêm mapping cho Update sau này
        // CreateMap<UpdateEquipmentCommand, Equipment>();
    }
}
