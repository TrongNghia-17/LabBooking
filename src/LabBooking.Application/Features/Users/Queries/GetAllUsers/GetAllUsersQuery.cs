using LabBooking.Application.Common.Wrappers;
using LabBooking.Application.Features.Users.Dtos;

namespace LabBooking.Application.Features.Users.Queries.GetAllUsers;

/// <summary>
/// Query để lấy tất cả người dùng.
/// Đã chuyển đổi thành record và bỏ giá trị mặc định (Validator sẽ xử lý).
/// </summary>
public record GetAllUsersQuery(
    // Tham số lọc
    string? SearchPhrase,
    string? RoleName,

    // Tham số phân trang (bắt buộc bởi Validator)
    int PageNumber,
    int PageSize,

    // Tham số sắp xếp
    string? SortBy,
    SortDirection SortDirection

) : IRequest<PagedResult<UserResponse>>;
