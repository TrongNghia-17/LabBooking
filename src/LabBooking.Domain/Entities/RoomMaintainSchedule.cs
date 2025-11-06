using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Entities
{
    public class RoomMaintainSchedule
    {
        public Guid Id { get; set; }
        public Guid LabRoomId { get; set; }
        [ForeignKey(nameof(LabRoomId))]
        public LabRoom? LabRoom { get; set; }
        public bool IsManyDay { get; set; }
        public bool? IsAllDay { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? NumberOfSlot { get; set; }
        public RoomMaintainStatus? RoomMaintainStatus { get; set; }
        public string? Description { get; set; }

    }
    public enum RoomMaintainStatus
    {
        Done,
        NotYet
    }
}
