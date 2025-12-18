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
        var userRoles = currentUserService.Roles; // Giả sử bạn lấy được Role (string)

        if (currentUserId == Guid.Empty) throw new UnauthorizedAccessException();

        Guid? filterManagerId = null;
        Guid? filterRequestedById = null;

        // --- PHÂN LOẠI NGƯỜI DÙNG ---
        if (userRoles != null && userRoles.Contains("Manager"))
        {
            // Nếu là Manager -> Lọc theo ManagerId (xem request người khác gửi cho mình)
            filterManagerId = currentUserId;
        }
        else // Student, Lecturer
        {
            // Nếu là User thường -> Lọc theo RequestedById (xem request của chính mình)
            filterRequestedById = currentUserId;
        }

        // Gọi Repo mới
        var (items, totalCount) = await doorRequestRepository.GetPagedListAsync(
            filterManagerId,     // ManagerId
            filterRequestedById, // RequestedById
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            request.FilterDate,
            request.FilterStatus,
            request.IsHistory,
            cancellationToken
        );

        var dtos = mapper.Map<IEnumerable<DoorRequestDto>>(items);

        return new PagedResult<DoorRequestDto>(
            dtos,
            totalCount,
            request.PageSize,
            request.PageNumber);
    }
}