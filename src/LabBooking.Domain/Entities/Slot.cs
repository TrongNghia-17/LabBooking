using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Entities
{
    public class Slot
    {
        public Guid Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int SlotIndex { get; set; } // Chỉ số của slot 1->4
        public string Label { get; set; }
    }
}
