namespace LabBooking.Infrastructure.Repositories;

internal class EquipmentCategoryRepository(LabBookingDbContext dbContext) : IEquipmentCategoryRepository
{
    public async Task<IEnumerable<EquipmentCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.EquipmentCategories
            .Include(c => c.Equipments)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Equipment>> GetEquipmentsByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Equipments
            .Include(e => e.LabRoom)
            .Where(e => e.EquipmentCategoryId == categoryId)
            .OrderBy(e => e.EquipmentName)
            .ToListAsync(cancellationToken);
    }
}