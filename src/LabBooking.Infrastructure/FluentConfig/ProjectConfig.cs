namespace LabBooking.Infrastructure.FluentConfig;

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

        builder.HasMany(p => p.Members)
               .WithMany() // simple many-to-many (ProjectMembers table auto-created)
               .UsingEntity(j => j.ToTable("ProjectMembers"));
    }
}
