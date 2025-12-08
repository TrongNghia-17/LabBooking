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

    public async Task<IEnumerable<EquipmentCategory>> GetByManagerIdAsync(Guid managerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.EquipmentCategories
            .Include(c => c.Equipments.Where(e => e.LabRoom.MainManagerId == managerId))
            .Where(c => c.Equipments.Any(e =>
                e.LabRoom != null &&
                e.LabRoom.MainManagerId == managerId))
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

    public async Task<IEnumerable<Equipment>> GetEquipmentsByCategoryAndManagerAsync(
        Guid categoryId,
        Guid managerId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Equipments
            .Include(e => e.LabRoom)
            .Where(e => e.EquipmentCategoryId == categoryId &&
                        e.LabRoom != null &&
                        e.LabRoom.MainManagerId == managerId)
            .OrderBy(e => e.EquipmentName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.EquipmentCategories
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<EquipmentCategory>> GetByLabIdAsync(Guid labId, CancellationToken cancellationToken = default)
    {
        return await dbContext.EquipmentCategories
            // 1. Chỉ include những thiết bị thuộc về LabId này
            .Include(c => c.Equipments.Where(e => e.LabRoomId == labId))
            // 2. Chỉ lấy những Category có chứa ít nhất 1 thiết bị thuộc LabId này
            .Where(c => c.Equipments.Any(e => e.LabRoomId == labId))
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }


}