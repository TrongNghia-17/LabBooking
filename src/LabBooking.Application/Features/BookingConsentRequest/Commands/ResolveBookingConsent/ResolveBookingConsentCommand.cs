using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingConsentRequest.Commands.ResolveBookingConsent
{
    public record ResolveBookingConsentCommand : IRequest<bool>
    {
        public Guid ConsentId { get; init; }
        public string Action { get; init; } = string.Empty; // "Cancel" hoặc "Reschedule"

        // Danh sách slot mới (chỉ dùng khi Action = "Reschedule")
        public List<NewSlotInput>? NewSlots { get; init; }
    }

    public record NewSlotInput(DateOnly Date, Guid SlotId);
}
