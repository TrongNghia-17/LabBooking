namespace LabBooking.Domain.Repositories;

public interface ISlotRepository
{
    Task<Slot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Slot>, int)> GetAllSlotAsync(CancellationToken cancellationToken = default);
    Task<Guid> Create(Slot entity, CancellationToken cancellationToken = default);
    Task<bool> IsSlotIndexUniqueAsync(int slotIndex, CancellationToken cancellationToken = default);
    Task DeleteAsync(Slot entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Slot entity, CancellationToken cancellationToken = default);
    Task<bool> IsSlotIndexUniqueAsync(Guid id, int slotIndex, CancellationToken cancellationToken = default);
}
