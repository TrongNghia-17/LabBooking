namespace LabBooking.Infrastructure.Repositories;

internal class UserRepository(LabBookingDbContext dbContext) : IUserRepository
{
    public async Task<(IEnumerable<User>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        string? roleName)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = dbContext.Users.AsNoTracking();

        // 1. LỌC THEO TỪ KHÓA (SearchPhrase)
        // Tìm kiếm theo UserName, Email, và Major
        baseQuery = baseQuery
            .Where(u => searchPhraseLower == null ||
                        (u.UserName != null && u.UserName.ToLower().Contains(searchPhraseLower)) ||
                        (u.Email != null && u.Email.ToLower().Contains(searchPhraseLower)) ||
                        (u.Major != null && u.Major.ToLower().Contains(searchPhraseLower))); //

        // 2. LỌC THEO ROLE (Logic mới)
        if (!string.IsNullOrEmpty(roleName))
        {
            // Tìm RoleId từ tên Role (chuyển về chữ hoa để so sánh)
            var roleId = await dbContext.Roles
                .Where(r => r.NormalizedName == roleName.ToUpper())
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (roleId != Guid.Empty)
            {
                // Lấy danh sách UserId trong Role đó từ bảng UserRoles
                var userIdsInRole = dbContext.UserRoles
                    .Where(ur => ur.RoleId == roleId)
                    .Select(ur => ur.UserId);

                // Lọc truy vấn chính để CHỈ bao gồm các user trong danh sách đó
                baseQuery = baseQuery.Where(u => userIdsInRole.Contains(u.Id));
            }
            else
            {
                // Nếu không tìm thấy Role, trả về 0 kết quả
                baseQuery = baseQuery.Where(u => false);
            }
        }

        // Lấy tổng số lượng (TRƯỚC KHI phân trang)
        var totalCount = await baseQuery.CountAsync();

        // 3. SẮP XẾP (Sorting)
        if (!string.IsNullOrEmpty(sortBy))
        {
            var columnsSelector = new Dictionary<string, Expression<Func<User, object?>>>
            {
                { nameof(User.UserName), u => u.UserName },
                { nameof(User.Email), u => u.Email },
                { nameof(User.Major), u => u.Major },
                { nameof(User.RegistrationDate), u => u.RegistrationDate }
            };

            // Chỉ sort nếu cột sortBy tồn tại trong dictionary
            if (columnsSelector.TryGetValue(sortBy, out var selectedColumn))
            {
                baseQuery = sortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }
        }

        // 4. PHÂN TRANG (Pagination)
        var users = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users.AnyAsync(u => u.Id == id, cancellationToken);
    }
}
