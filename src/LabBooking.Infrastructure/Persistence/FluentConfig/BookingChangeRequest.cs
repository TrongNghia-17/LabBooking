using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Infrastructure.Persistence.FluentConfig
{
    public class BookingChangeRequestConfig : IEntityTypeConfiguration<LabBooking.Domain.Entities.BookingChangeRequest>
    {
        public void Configure(EntityTypeBuilder<LabBooking.Domain.Entities.BookingChangeRequest> builder)
        {
            builder.HasOne(bcr => bcr.CreatedBy)
                    .WithMany()
                    .HasForeignKey(bcr => bcr.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bcr => bcr.ApprovedBy)
                .WithMany()
                .HasForeignKey(bcr => bcr.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
