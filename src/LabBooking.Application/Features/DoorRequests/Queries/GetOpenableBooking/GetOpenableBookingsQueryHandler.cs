using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetOpenableBooking;

public class GetOpenableBookingsQueryHandler(
    IDoorRequestRepository doorRequestRepository
    ) : IRequestHandler<GetOpenableBookingsQuery, List<BookingForDoorOpenDto>>
{
    public async Task<List<BookingForDoorOpenDto>> Handle(GetOpenableBookingsQuery request, CancellationToken cancellationToken)
    {
        // Gọi Repo đã viết ở bước 2
        var validSlots = await doorRequestRepository.GetBookingsEligibleForDoorOpenAsync(request.UserId);

        // Map từ Entity sang DTO
        var result = validSlots.Select(s => new BookingForDoorOpenDto
        {
            BookingId = s.BookingId,
            LabRoomId = s.Booking.LabRoomId,
            LabRoomName = s.Booking.LabRoom!.LabName ?? "Phòng Lab",
            SlotName = s.Slot?.Label ?? "N/A",
            StartTime = s.Slot!.StartTime,
            EndTime = s.Slot!.EndTime,
            Date = s.Date
        }).ToList();

        // (Optional) Nếu user book 2 slot liên tiếp (Ca 1 + Ca 2) cùng 1 phòng, 
        // bạn có thể muốn GroupBy LabRoomId để chỉ hiện 1 nút bấm. 
        // Nhưng logic dưới đây sẽ trả về từng slot để user biết rõ.

        return result;
    }
}
