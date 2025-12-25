using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.RoomChecks.Dtos;

namespace LabBooking.Application.Features.RoomChecks.Queries.GetRoomChecks;

public class GetRoomChecksQuery : IRequest<PagedResult<RoomCheckDto>>
{
    // 1. Phân trang
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // 2. Tìm kiếm (Theo tên phòng, ghi chú...)
    public string? SearchPhrase { get; set; }

    // 3. Lọc theo loại (CheckIn / CheckOut)
    public CheckType? Type { get; set; }

    // 4. Lọc theo ngày (Từ ngày -> Đến ngày)
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
