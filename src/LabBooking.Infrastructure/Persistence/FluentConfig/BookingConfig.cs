namespace LabBooking.Application.Persistence.FluentConfig;

public class BookingConfig : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        //name of table

        //name of columns       

        //primary key

        //other validations

        //relations
        builder.HasOne(b => b.LabRoom)
               .WithMany()
               .HasForeignKey(b => b.LabRoomId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Slots)
               .WithOne(s => s.Booking)
               .HasForeignKey(s => s.BookingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Participants)
               .WithOne(p => p.Booking)
               .HasForeignKey(p => p.BookingId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
