namespace LabBooking.Infrastructure.Repositories;

internal class IncidentRepository(LabBookingDbContext dbContext) : IIncidentRepository
{
    public async Task<IEnumerable<Incident>> GetAllAsync()
    {
        var incidents = await dbContext.Incidents.ToListAsync();
        return incidents;
    }
}
