namespace LabBooking.Domain.Repositories;

public interface ILabRoomRepository
{
    Task<Guid> Create(LabRoom entity);
    Task<LabRoom?> GetByIdAsync(Guid id);
    Task Update(LabRoom entity);
    Task DeleteAsync(LabRoom entity);
    Task<bool> IsLabNameUniqueAsync(string labName);
    Task<(IEnumerable<LabRoom>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection);
}
