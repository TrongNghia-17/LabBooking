using Microsoft.EntityFrameworkCore;

namespace LabBooking.Infrastructure.Persistence.FluentConfig;

public class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        //name of table

        //name of columns       
        builder.HasIndex(n => n.UserId);

        //primary key

        //other validations

        //relations
        builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
    }
}
