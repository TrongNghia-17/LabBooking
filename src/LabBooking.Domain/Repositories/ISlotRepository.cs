using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Domain.Repositories
{
    public interface ISlotRepository
    {
        Task<(IEnumerable<Slot>, int)> GetAllSlotAsync(CancellationToken cancellationToken = default);
    }
}
