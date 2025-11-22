using LabBooking.Application.Features.Slots.Commands.CreateSlot;
using LabBooking.Application.Features.Slots.Commands.UpdateSlot;

namespace LabBooking.Application.Features.Slots.Dtos
{
    public class SlotProfile : Profile
    {
        public SlotProfile()
        {
            CreateMap<CreateSlotCommand, Slot>();
            CreateMap<UpdateSlotCommand, Slot>();
            CreateMap<Slot, SlotResponse>();
        }
    }
}
