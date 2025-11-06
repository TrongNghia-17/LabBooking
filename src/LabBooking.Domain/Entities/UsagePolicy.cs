using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Entities
{
    public class UsagePolicy
    {
        public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public User? CreatedBy { get; set; }
        public bool ForAllLabRooms { get; set; } = false;
        public Guid? LabRoomId { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
