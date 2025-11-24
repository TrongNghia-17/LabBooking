using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Entities
{
    public class ExternalEquipment
    {
        public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();
        public string? EquipmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public Guid BookingId { get; set; }
        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }
    }
}
