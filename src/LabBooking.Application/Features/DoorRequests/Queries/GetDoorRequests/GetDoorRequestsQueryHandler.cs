using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.DoorRequests.Dtos;

namespace LabBooking.Application.Features.DoorRequests.Queries.GetDoorRequests;

public class GetDoorRequestsQueryHandler(
    IDoorRequestRepository doorRequestRepository,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IRequestHandler<GetDoorRequestsQuery, PagedResult<DoorRequestDto>>
{
    public async Task<PagedResult<DoorRequestDto>> Handle(GetDoorRequestsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;
        if (currentUserId == Guid.Empty) throw new UnauthorizedAccessException();

        // Gọi Repo để lấy dữ liệu
        var (items, totalCount) = await doorRequestRepository.GetRequestsByManagerAsync(
            currentUserId!.Value, // Chỉ lấy của Manager đang đăng nhập
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            request.FilterDate,
            request.FilterStatus,
            cancellationToken
        );

        // Map sang DTO
        var dtos = mapper.Map<IEnumerable<DoorRequestDto>>(items);

        // Trả về kết quả phân trang
        return new PagedResult<DoorRequestDto>(
            dtos,
            totalCount,
            request.PageSize,
            request.PageNumber);
    }
}