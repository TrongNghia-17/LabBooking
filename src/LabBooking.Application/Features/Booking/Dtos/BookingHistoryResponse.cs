using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.Booking.Dtos
{
    public class BookingHistoryResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public BookingStatus? Status { get; set; } // 1 = Approved

        // Flatten dữ liệu LabRoom ra cho gọn
        public BookingHistoryLabDto? LabRoom { get; set; }

        public List<BookingHistorySlotDto> Slots { get; set; } = new();
    }

    public class BookingHistoryLabDto
    {
        public Guid Id { get; set; }
        public string LabName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class BookingHistorySlotDto
    {
        public Guid Id { get; set; }
        public Guid SlotId { get; set; }
        public DateOnly Date { get; set; }
        public int Status { get; set; } // 0 = Active
    }
}
