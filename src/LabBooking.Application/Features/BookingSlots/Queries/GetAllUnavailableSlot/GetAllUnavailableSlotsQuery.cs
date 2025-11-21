using LabBooking.Application.Features.BookingSlots.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Application.Features.BookingSlots.Queries.GetAllUnavailableSlot
{
    public record GetAllUnavailableSlotsQuery(
        Guid LabRoomId,
        DateOnly StartDate, // FE gửi ngày Thứ 2
        DateOnly EndDate    // FE gửi ngày Chủ Nhật
    ) : IRequest<IEnumerable<BookingSlotResponse>>;
}
