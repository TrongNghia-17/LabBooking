using LabBooking.Application.Features.BookingSlots.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingSlots.Queries.GetAllUnavailableSlot
{
    public class GetAllUnavailableSlotsQuery : IRequest<IEnumerable<BookingSlotResponse>>
    {
        public Guid LabRoomId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        // Phải có dòng này thì ở Controller mới gán được
        public Guid? CurrentUserId { get; set; }
    }
}
