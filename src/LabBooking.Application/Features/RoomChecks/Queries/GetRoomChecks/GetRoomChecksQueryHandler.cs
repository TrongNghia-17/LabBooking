using LabBooking.Application.Common.Interfaces;
using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.RoomChecks.Dtos;

namespace LabBooking.Application.Features.RoomChecks.Queries.GetRoomChecks;

public class GetRoomChecksQueryHandler(
    IRoomCheckRepository roomCheckRepository,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IRequestHandler<GetRoomChecksQuery, PagedResult<RoomCheckDto>>
{
    public async Task<PagedResult<RoomCheckDto>> Handle(GetRoomChecksQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy thông tin User
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
        var roles = currentUserService.Roles;

        // 2. Xác định quyền: Là Manager hay là Bảo vệ?
        // Nếu là Admin thì có thể xem hết (tùy bạn, ở đây tôi set như Manager xem hết)
        bool isManager = roles.Contains("Manager") || roles.Contains("Admin");

        // 3. Gọi Repo
        var (items, totalCount) = await roomCheckRepository.GetPagedListAsync(
            userId,
            isManager,
            request.SearchPhrase,
            request.Type,
            request.FromDate,
            request.ToDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        // 4. Map sang DTO
        var dtos = mapper.Map<IEnumerable<RoomCheckDto>>(items);

        return new PagedResult<RoomCheckDto>(
            dtos,
            totalCount,
            request.PageSize,
            request.PageNumber);
    }
}