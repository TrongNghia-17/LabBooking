namespace LabBooking.Infrastructure.Persistence;

internal class LabBookingDbContext(DbContextOptions<LabBookingDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingChangeRequest> BookingChangeRequests { get; set; }
    public DbSet<BookingParticipant> BookingParticipants { get; set; }
    public DbSet<BookingSlot> BookingSlots { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<DoorOpeningRequest> DoorOpeningRequests { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<EquipmentMaintainSchedule> EquipmentMaintainSchedules { get; set; }
    public DbSet<ExternalEquipment> ExternalEquipments { get; set; }
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<LabRoom> LabRooms { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<OutSideGuest> OutSideGuests { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<RoomMaintainSchedule> RoomMaintainSchedules { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<Support> Supports { get; set; }
    public DbSet<UsagePolicy> UsagePolicies { get; set; }
    public DbSet<UserDevice> UserDevices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
