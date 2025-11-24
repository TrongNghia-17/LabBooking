using LabBooking.Application.Features.Equipments.Dtos;
using LabBooking.Application.Features.LabRooms.Commands.CreateLabRoom;
using LabBooking.Application.Features.LabRooms.Commands.UpdateLabRoom;

namespace LabBooking.Application.Features.LabRooms.Dtos;

public class LabRoomProfile : Profile
{
    public LabRoomProfile()
    {
        // Mapping cho Create
        CreateMap<CreateLabRoomCommand, LabRoom>();
        CreateMap<LabRoom, LabRoomResponse>();

        // Mapping cho Update
        CreateMap<UpdateLabRoomCommand, LabRoom>();

        // Mapping cho Equipment
        CreateMap<Equipment, EquipmentResponse>();
    }
}
