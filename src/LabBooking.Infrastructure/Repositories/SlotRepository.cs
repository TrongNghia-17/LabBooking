using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Infrastructure.Repositories
{
    internal class SlotRepository(LabBookingDbContext dbContext) : ISlotRepository
    {
        public async Task<(IEnumerable<Slot>, int)> GetAllSlotAsync(CancellationToken cancellationToken = default)
        {
            var baseQuery = dbContext.Slots.AsQueryable();

            var totalCount = await baseQuery.CountAsync();

            var slots = await baseQuery
                .ToListAsync(cancellationToken);

            return (slots, totalCount);
        }
    }
}
