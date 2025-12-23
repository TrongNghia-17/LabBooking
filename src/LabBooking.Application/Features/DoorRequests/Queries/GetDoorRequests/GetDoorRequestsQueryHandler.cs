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
        bool isManager = false; // Cờ đánh dấu

        // --- PHÂN LOẠI NGƯỜI DÙNG ---
        if (userRoles != null && userRoles.Contains("Manager"))
        {
            // Nếu là Manager -> Lọc theo ManagerId (xem request người khác gửi cho mình)
            filterManagerId = currentUserId;
            isManager = true;
        }
        else // Student, Lecturer
        {
            // Nếu là User thường -> Lọc theo RequestedById (xem request của chính mình)
            filterRequestedById = currentUserId;
            isManager = false;
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

        // 1. Map cơ bản bằng AutoMapper trước
        var dtos = mapper.Map<List<DoorRequestDto>>(items);

        // 2. Xử lý hiển thị thông tin đối ứng (Contact Info)
        // DTO field: RequestedByName đang được hiểu là "Tên người liên hệ"
        for (int i = 0; i < dtos.Count; i++)
        {
            var itemEntity = items.ElementAt(i);
            var dto = dtos[i];

            if (isManager)
            {
                // Nếu là Manager xem -> Giữ nguyên (Xem thông tin người gửi - SV)
                // (AutoMapper đã làm việc này rồi, nhưng gán lại cho chắc hoặc để minh họa logic)
                dto.ContactName = itemEntity.RequestedBy?.FullName ?? "Unknown Student";
                dto.RequestedByEmail = itemEntity.RequestedBy?.Email;
                dto.RequestedByPhoneNumber = itemEntity.RequestedBy?.PhoneNumber ?? "N/A";
            }
            else
            {
                // Nếu là Student xem -> ĐỔI thành thông tin Manager
                dto.ContactName = itemEntity.Manager?.FullName ?? "Unknown Manager";
                dto.RequestedByEmail = itemEntity.Manager?.Email;
                dto.RequestedByPhoneNumber = itemEntity.Manager?.PhoneNumber ?? "N/A";
            }
        }

        return new PagedResult<DoorRequestDto>(
            dtos,
            totalCount,
            request.PageSize,
            request.PageNumber);
    }
}