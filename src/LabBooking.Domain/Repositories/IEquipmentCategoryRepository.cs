namespace LabBooking.Domain.Repositories;

public interface IEquipmentCategoryRepository
{
    Task<EquipmentCategory?> GetByIdAsync(Guid id, CancellationToken token);
    Task UpdateAsync(EquipmentCategory category, CancellationToken token);

    // Kiểm tra tên tồn tại (trừ ID hiện tại ra)
    Task<bool> IsNameExistsExcludeIdAsync(string name, Guid excludeId, CancellationToken token);
    Task<Guid> CreateAsync(EquipmentCategory category, CancellationToken token);
    Task<bool> IsNameExistsAsync(string name, CancellationToken token); // <--- Thêm hàm này
                                                                        // Trong file IEquipmentCategoryRepository.cs
    Task<(IEnumerable<EquipmentCategory>, int)> GetAllMatchingAsync(
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection,
        Guid? userId,      // <--- Thêm tham số này
        bool isAdmin,      // <--- Thêm tham số này
        CancellationToken cancellationToken);
    Task<IEnumerable<Equipment>> GetEquipmentsByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<EquipmentCategory>> GetByManagerIdAsync(Guid managerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Equipment>> GetEquipmentsByCategoryAndManagerAsync(
            Guid categoryId,
            Guid managerId,
            CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EquipmentCategory>> GetByLabIdAsync(Guid labId, CancellationToken cancellationToken = default);
}
