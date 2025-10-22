namespace LabBooking.Application.Persistence.FluentConfig;

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
    }
}
