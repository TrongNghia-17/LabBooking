namespace LabBooking.Domain.Repositories;

public interface IEquipmentCategoryRepository
{
    Task<IEnumerable<EquipmentCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Equipment>> GetEquipmentsByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<EquipmentCategory>> GetByManagerIdAsync(Guid managerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Equipment>> GetEquipmentsByCategoryAndManagerAsync(
            Guid categoryId,
            Guid managerId,
            CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
