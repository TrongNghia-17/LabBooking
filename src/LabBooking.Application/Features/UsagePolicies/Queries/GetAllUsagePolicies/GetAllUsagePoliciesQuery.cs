using LabBooking.Application.Features.UsagePolicies.Dtos;

namespace LabBooking.Application.Features.UsagePolicies.Queries.GetAllUsagePolicies;

/// <summary>
/// Record chứa các tham số để truy vấn danh sách UsagePolicy (có phân trang, tìm kiếm, sắp xếp, lọc).
/// </summary>
public record GetAllUsagePoliciesQuery(
    // Tham số tìm kiếm (giống LabRoom)
    string? SearchPhrase,

    // Tham số lọc mới
    bool? IsActive,

    // Tham số phân trang (giống LabRoom)
    int PageNumber,
    int PageSize,

    // Tham số sắp xếp (giống LabRoom)
    string? SortBy,
    SortDirection SortDirection
) : IRequest<PagedResult<UsagePolicyResponse>>;
