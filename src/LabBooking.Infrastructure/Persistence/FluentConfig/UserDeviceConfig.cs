namespace LabBooking.Infrastructure.Persistence.FluentConfig;

public class UserDeviceConfig : IEntityTypeConfiguration<UserDevice>
{
    public void Configure(EntityTypeBuilder<UserDevice> builder)
    {
        builder
            .HasIndex(d => d.PushToken)
            .IsUnique();

        builder
            .HasOne(d => d.User)
            .WithMany(u => u.UserDevices)
            .HasForeignKey(d => d.UserId);
    }
}
