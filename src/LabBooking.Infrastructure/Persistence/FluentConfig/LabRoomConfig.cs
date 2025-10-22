namespace LabBooking.Application.Persistence.FluentConfig;

public class LabRoomConfig : IEntityTypeConfiguration<LabRoom>
{
    public void Configure(EntityTypeBuilder<LabRoom> builder)
    {
        //name of table

        //name of columns       

        //primary key

        //other validations

        //relations
        builder.HasOne(l => l.MainManager)
               .WithMany()
               .HasForeignKey(l => l.MainManagerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.Equipments)
               .WithOne(e => e.LabRoom)
               .HasForeignKey(e => e.LabRoomId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.Incidents)
               .WithOne(i => i.LabRoom)
               .HasForeignKey(i => i.LabRoomId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
