namespace LabBooking.Infrastructure.Persistence.FluentConfig;

public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        //name of table
        builder.ToTable("RefreshTokens");

        //name of columns 
        builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(512);

        builder.Property(rt => rt.Expires)
                .IsRequired();

        //primary key
        builder.HasKey(rt => rt.Id);

        //other validations
        builder.Ignore(rt => rt.IsExpired);
        builder.Ignore(rt => rt.IsRevoked);
        builder.Ignore(rt => rt.IsActive);

        //relations
        builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
    }
}
