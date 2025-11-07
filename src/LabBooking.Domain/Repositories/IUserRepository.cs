namespace LabBooking.Domain.Repositories;

public interface IUserRepository
{
    Task<(IEnumerable<User>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        string? roleName
    );
}
