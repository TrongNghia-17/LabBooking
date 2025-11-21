namespace LabBooking.Domain.Repositories;

public interface IEquipmentRepository
{
    Task<Guid> Create(Equipment entity);
    Task<Equipment?> GetByIdAsync(Guid id);
    Task Update(Equipment entity);
    Task DeleteAsync(Equipment entity);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Equipment>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection);
}
