namespace LabBooking.Application.Persistence;

internal class LabBookingDbContext(DbContextOptions<LabBookingDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingParticipant> BookingParticipants { get; set; }
    public DbSet<BookingSlot> BookingSlots { get; set; }
    public DbSet<DoorRequest> DoorRequests { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<LabRoom> LabRooms { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
