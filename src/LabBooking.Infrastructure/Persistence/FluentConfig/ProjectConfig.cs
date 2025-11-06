namespace LabBooking.Infrastructure.Persistence.FluentConfig;

public class ProjectConfig : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        //name of table

        //name of columns       

        //primary key

        //other validations

        //relations
        builder.HasOne(p => p.Owner)
               .WithMany()
               .HasForeignKey(p => p.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);

    }
}
