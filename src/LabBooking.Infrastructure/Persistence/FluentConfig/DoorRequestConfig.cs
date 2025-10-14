namespace LabBooking.Infrastructure.Persistence.FluentConfig;

public class DoorRequestConfig : IEntityTypeConfiguration<DoorRequest>
{
    public void Configure(EntityTypeBuilder<DoorRequest> builder)
    {
        //name of table

        //name of columns       

        //primary key

        //other validations

        //relations
        builder.HasOne(d => d.RequestedBy)
               .WithMany()
               .HasForeignKey(d => d.RequestedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.HandledBy)
               .WithMany()
               .HasForeignKey(d => d.HandledById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.LabRoom)
               .WithMany()
               .HasForeignKey(d => d.LabRoomId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
