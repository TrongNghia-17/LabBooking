namespace LabBooking.Infrastructure.FluentConfig;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        //name of table

        //name of columns       

        //primary key

        //other validations

        //relations  
        builder.HasMany(u => u.Notifications)
               .WithOne(n => n.User)
               .HasForeignKey(n => n.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Bookings)
               .WithOne(b => b.CreatedBy)
               .HasForeignKey(b => b.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Incidents)
               .WithOne(i => i.ReportedBy)
               .HasForeignKey(i => i.ReportedById)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
