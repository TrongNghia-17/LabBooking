using Microsoft.EntityFrameworkCore;

namespace LabBooking.Infrastructure.Persistence.FluentConfig;

public class BookingConfig : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        //name of table

        //name of columns       

        //primary key

        //other validations
        builder.Property(b => b.Status).HasConversion<string>();
        builder.Property(b => b.Type).HasConversion<string>();
        builder.Property(b => b.Priority).HasConversion<string>();
        //relations
        builder.HasOne(b => b.LabRoom)
               .WithMany(r => r.Bookings)
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
