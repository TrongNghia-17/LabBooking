namespace LabBooking.Infrastructure.Repositories;

internal class EquipmentRepository(LabBookingDbContext dbContext) : IEquipmentRepository
{
    public async Task<Guid> Create(Equipment entity)
    {
        dbContext.Equipments.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public Task DeleteAsync(Equipment entity)
    {
        throw new NotImplementedException();
    }

    public Task<Equipment?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task Update(Equipment entity)
    {
        throw new NotImplementedException();
    }
}
