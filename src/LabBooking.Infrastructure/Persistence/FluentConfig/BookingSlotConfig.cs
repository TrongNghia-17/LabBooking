namespace LabBooking.Infrastructure.Persistence.FluentConfig;

public class BookingSlotConfig : IEntityTypeConfiguration<BookingSlot>
{
    public void Configure(EntityTypeBuilder<BookingSlot> builder)
    {
        //name of table

        //name of columns       
        builder.HasIndex(s => new { s.Date, s.SlotIndex, s.BookingId }).IsUnique();

        //primary key

        //other validations

        //relations
    }
}
