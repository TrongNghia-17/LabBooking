using LabBooking.Application.Features.Booking.Dtos;

namespace LabBooking.Application.Features.Booking.Queries.GetTimetable
{
    public class GetTimetableQueryHandler(IBookingRepository bookingRepository,
        IMapper mapper
        ) : IRequestHandler<GetTimetableQuery, List<BookingHistoryResponse>>
    {
        public async Task<List<BookingHistoryResponse>> Handle(GetTimetableQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy ID user hiện tại
            var userId = request.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("Bạn cần đăng nhập để xem lịch sử.");

            // 2. Gọi Repo lấy dữ liệu
            var bookings = await bookingRepository.GetApprovedHistoryByUserIdAsync(userId.Value, cancellationToken);

            // 3. Map sang DTO
            var bookingResponses = bookings.Select(b => new BookingHistoryResponse
            {
                Id = b.Id,
                Title = b.Title ?? "No Title", // Xử lý null nếu cần
                Status = b.Status, // Ép kiểu Enum sang int để khớp DTO

                // Map object con: LabRoom
                LabRoom = b.LabRoom != null ? new BookingHistoryLabDto
                {
                    Id = b.LabRoom.Id,
                    LabName = b.LabRoom.LabName,
                    Location = b.LabRoom.Location
                } : null,

                // Map list con: Slots
                Slots = b.Slots?.Select(s => new BookingHistorySlotDto
                {
                    Id = s.Id,
                    SlotId = s.SlotId,
                    Date = s.Date,
                    Status = (int)s.Status // Ép kiểu Enum BookingSlotStatus sang int
                }).ToList() ?? new List<BookingHistorySlotDto>()

            }).ToList();
            return bookingResponses;
        }
    }
}
