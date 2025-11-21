using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingSlots.Dtos
{
    public class BookingSlotProfile : Profile
    {
        public BookingSlotProfile()
        {
            CreateMap<BookingSlot, BookingSlotResponse>().ReverseMap();
        }
    }
}
