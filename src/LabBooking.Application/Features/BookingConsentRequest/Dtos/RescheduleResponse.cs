using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingConsentRequest.Dtos
{
    public class RescheduleResponse
    {
        public Guid BookingId { get; set; }
        public string BookingTitle { get; set; }
        public Guid LabId { get; set; }
        public string LabName { get; set; }
        public int SlotDebtCount { get; set; }
        public List<SimpleSlotDto> LostSlots { get; set; } = new();
        public List<SimpleSlotDto> SafeSlots { get; set; } = new();
    }

    public class SimpleSlotDto
    {
        public Guid SlotId { get; set; }
        public string Date { get; set; }
    }

    public class OverriddenSlotJsonModel
    {
        public Guid BookingSlotId { get; set; } // Đây là cái ID của slot trong booking (cái bạn cần check)
        public Guid SlotId { get; set; }        // ID của Template Slot
        public string Date { get; set; }
    }
}
