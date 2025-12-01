namespace LabBooking.Domain.Repositories;

public interface IEquipmentCategoryRepository
{
    Task<IEnumerable<EquipmentCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Equipment>> GetEquipmentsByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
