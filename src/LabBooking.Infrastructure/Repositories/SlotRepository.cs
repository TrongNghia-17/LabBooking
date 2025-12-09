namespace LabBooking.Infrastructure.Repositories
{
    internal class SlotRepository(LabBookingDbContext dbContext) : ISlotRepository
    {
        public async Task<(IEnumerable<Slot>, int)> GetAllSlotAsync(CancellationToken cancellationToken = default)
        {
            var baseQuery = dbContext.Slots.AsQueryable();

            var totalCount = await baseQuery.CountAsync();

            var slots = await baseQuery
                .OrderBy(s => s.StartTime)
                .ToListAsync(cancellationToken);

            return (slots, totalCount);
        }

        public async Task<bool> IsSlotIndexUniqueAsync(int slotIndex, CancellationToken cancellationToken = default)
        {
            return !await dbContext.Slots
                .AnyAsync(s => s.SlotIndex == slotIndex, cancellationToken);
        }

        public async Task<Guid> Create(Slot entity, CancellationToken cancellationToken = default)
        {
            dbContext.Slots.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        public async Task UpdateAsync(Slot entity, CancellationToken cancellationToken = default)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        // Check trùng SlotIndex nhưng BỎ QUA record có ID đang sửa
        public async Task<bool> IsSlotIndexUniqueAsync(Guid id, int slotIndex, CancellationToken cancellationToken = default)
        {
            var isDuplicate = await dbContext.Slots
                .AnyAsync(s => s.SlotIndex == slotIndex && s.Id != id, cancellationToken);

            return !isDuplicate;
        }

        public async Task<Slot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Slots.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task DeleteAsync(Slot entity, CancellationToken cancellationToken = default)
        {
            dbContext.Slots.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
