using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Infrastructure.Persistence.FluentConfig
{
    public class IncidentConfig : IEntityTypeConfiguration<Incident>
    {
        public void Configure(EntityTypeBuilder<Incident> builder)
        {
            builder.HasOne(i => i.ReportedBy)
                .WithMany(u => u.Incidents) // Thêm ICollection<Incident> Incidents vào User
                .HasForeignKey(i => i.ReportedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
