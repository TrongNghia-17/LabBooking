using LabBooking.Application.Features.LabRooms.Dtos;

namespace LabBooking.Domain.Repositories;

public interface ILabRoomRepository
{
    Task<Guid> Create(LabRoom entity, CancellationToken cancellationToken = default);
    Task<LabRoom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task Update(LabRoom entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(LabRoom entity, CancellationToken cancellationToken = default);
    Task<bool> IsLabNameUniqueAsync(string labName, CancellationToken cancellationToken = default);
    Task<bool> IsLabNameUniqueAsync(Guid id, string labName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<LabRoom>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection, CancellationToken cancellationToken = default);
    Task<IEnumerable<LabRoom>> GetUnmaintainedLabRoomsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<LabRoom>> GetLabsByManagerWithEquipmentsAsync(Guid managerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MonthlyTopLabDto>> GetTopLabPerMonthAsync(int year, CancellationToken token);
}
