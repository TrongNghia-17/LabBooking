namespace LabBooking.Infrastructure.Seeders;

internal class LabBookingSeeder(LabBookingDbContext dbContext) : ILabBookingSeeder
{
    public async Task Seed()
    {
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            await dbContext.Database.MigrateAsync();
        }
    }
}
