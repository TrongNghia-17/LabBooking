using Medo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.ExternalEquipment.Dtos
{
    public class ExternalEquipmentResponse
    {
        public Guid Id { get; set; } 
        public string? EquipmentName { get; set; } 
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public Guid BookingId { get; set; }
    }
}
