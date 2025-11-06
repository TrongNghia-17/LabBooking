using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Entities
{
    public class EquipmentMaintainSchedule
    {
        public Guid Id { get; set; }
        public Guid EquipmentId { get; set; }
        [ForeignKey(nameof(EquipmentId))]
        public Equipment? Equipment { get; set; }
        public bool IsManyDay { get; set; }
        public bool? IsAllDay { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? NumberOfSlot { get; set; }
        public EquimentpMaintainStatus? EquimentpMaintainStatus { get; set; }
        public string? Description { get; set; }
        public int? NumberOf { get; set; }
    }
    public enum EquimentpMaintainStatus
    {
        Done,
        NotYet
    }
}
