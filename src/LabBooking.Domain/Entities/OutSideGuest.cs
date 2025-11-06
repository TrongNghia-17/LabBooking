using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Entities
{
    public class OutSideGuest
    {
        public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();
        public string? FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Organization { get; set; }
        public string? PurposeOfVisit { get; set; }
        public DateTime VisitDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public User? CreatedBy { get; set; }
    }
}
