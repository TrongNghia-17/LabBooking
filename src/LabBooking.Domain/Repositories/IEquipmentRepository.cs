namespace LabBooking.Domain.Repositories;

public interface IEquipmentRepository
{
    Task<Guid> Create(Equipment entity);
    Task<Equipment?> GetByIdAsync(Guid id);
    Task Update(Equipment entity);
    Task DeleteAsync(Equipment entity);
}
