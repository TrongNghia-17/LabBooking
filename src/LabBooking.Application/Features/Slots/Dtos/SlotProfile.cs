using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Slots.Dtos
{
    public class SlotProfile : Profile
    {
        public SlotProfile()
        {
            CreateMap<Slot, SlotResponse>();
        }
    }
}
