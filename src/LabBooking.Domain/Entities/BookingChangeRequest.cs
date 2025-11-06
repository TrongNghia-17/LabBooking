using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Entities
{
    public class BookingChangeRequest
    {
        public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();
        public Guid BookingId { get; set; }
        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        public Guid NewBookingSlotId { get; set; }
        [ForeignKey(nameof(NewBookingSlotId))]
        public BookingSlot? NewBookingSlot { get; set; }

        public Guid OldBookingSlotId { get; set; }
        [ForeignKey(nameof(OldBookingSlotId))]
        public BookingSlot? OldBookingSlot { get; set; }

        public Guid CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public User? CreatedBy { get; set; }

        public Guid ApprovedById { get; set; }
        [ForeignKey(nameof(ApprovedById))]
        public User? ApprovedBy { get; set; }

        public string? Reason { get; set; }
        public BookingChangeRequestStatus? Status { get; set; } = BookingChangeRequestStatus.Pending;
        public string? Type { get; set; }
    }

    public enum BookingChangeRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
