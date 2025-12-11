using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Application.Features.LabRooms.Queries.GetAllLabRooms;

public class GetAllLabRoomsQueryHandler(
    ILogger<GetAllLabRoomsQueryHandler> logger,
    ILabRoomRepository labRoomRepository,
    IBookingRepository bookingRepository,
    IMapper mapper) : IRequestHandler<GetAllLabRoomsQuery, PagedResult<LabRoomResponse>>
{
    public async Task<PagedResult<LabRoomResponse>> Handle(GetAllLabRoomsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all lab rooms");

        var (labRooms, totalCount) = await labRoomRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        var labRoomsResponse = mapper.Map<IEnumerable<LabRoomResponse>>(labRooms).ToList();

        if (request.FilterDate.HasValue && request.FilterSlotId.HasValue)
        {
            // Lấy tất cả các BookingSlot trùng với ngày và slot được chọn
            // (Bạn cần viết hàm GetBookedLabIdsAsync trong BookingRepository hoặc dùng DbContext trực tiếp nếu repo generic)
            var bookedLabIds = await bookingRepository.GetBookedLabIdsAsync(
                request.FilterDate.Value,
                request.FilterSlotId.Value,
                cancellationToken);

            foreach (var lab in labRoomsResponse)
            {
                // Nếu ID phòng nằm trong danh sách đã đặt -> Đánh dấu là Bận
                if (bookedLabIds.Contains(lab.Id))
                {
                    lab.Status = "Booked";
                }

                // Kiểm tra thêm nếu phòng đang IsActive = false thì set là Maintenance
                var originalLab = labRooms.FirstOrDefault(x => x.Id == lab.Id);
                if (originalLab != null && !originalLab.IsActive)
                {
                    lab.Status = "Maintenance";
                }
            }
        }

        var result = new PagedResult<LabRoomResponse>(
            labRoomsResponse,
            totalCount,
            request.PageSize,
            request.PageNumber);

        return result;
    }
}
