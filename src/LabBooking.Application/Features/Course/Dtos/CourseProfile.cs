using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Course.Dtos
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Domain.Entities.Course, CourseResponse>().ReverseMap();
        }
    }
}
