using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.ExternalEquipment.Dtos
{
    public class ExternalEquipmentProfile : Profile
    { 
        public ExternalEquipmentProfile() 
        {
            CreateMap<Domain.Entities.ExternalEquipment, ExternalEquipmentResponse>().ReverseMap();
        }
    }
}
