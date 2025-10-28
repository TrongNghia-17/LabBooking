namespace LabBooking.Infrastructure.Persistence.FluentConfig;

public class RoleConfig : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.ToTable("AspNetRoles");

        const string STUDENT_ROLE_ID = "fab4fac1-c546-41de-aebc-a14da6895711";
        const string TEACHER_ROLE_ID = "c7b013f0-5201-4317-abd8-c211f91b7330";

        // Seed Roles
        builder.HasData(
            new IdentityRole<Guid>
            {
                Id = Guid.Parse(STUDENT_ROLE_ID),
                Name = "Student",
                NormalizedName = "STUDENT",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse(TEACHER_ROLE_ID),
                Name = "Teacher",
                NormalizedName = "TEACHER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );
    }
}
