using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingPriorityDetail.Dtos
{
    public class BookingPriorityDetailProfile : Profile
    {
        public BookingPriorityDetailProfile()
        {
            CreateMap<Domain.Entities.BookingPriorityDetail, BookingPriorityDetailResponse>().ReverseMap();
        }
    }
}
