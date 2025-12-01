using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabBooking.Infrastructure.Persistence.FluentConfig
{
    public class BookingConsentRequestConfig : IEntityTypeConfiguration<BookingConsentRequest>
    {
        public void Configure(EntityTypeBuilder<BookingConsentRequest> builder)
        {
            // 1. Name of table
            builder.ToTable("BookingConsentRequests");

            // 2. Primary Key
            builder.HasKey(c => c.Id);

            // 3. Other validations & Conversions
            builder.Property(c => c.Status)
                   .HasConversion<string>() // Lưu enum dưới dạng chữ (Pending, Accepted...)
                   .IsRequired();

            builder.Property(c => c.OverriddenSlotIdsJson)
                   .HasColumnType("text") // Hoặc nvarchar(max) tùy DB
                   .IsRequired(false);

            // 4. Relations

            // Quan hệ 1: Booking bị đè (Nạn nhân)
            builder.HasOne(c => c.Booking)
                   .WithMany() // Bên Booking không cần giữ danh sách Consent
                   .HasForeignKey(c => c.BookingId)
                   .OnDelete(DeleteBehavior.Restrict);
            // Restrict: Xóa Booking cha KHÔNG tự động xóa Consent (để tránh lỗi vòng lặp khóa ngoại)

            // Quan hệ 2: Booking ưu tiên (Thủ phạm)
            builder.HasOne(c => c.PriorityBooking)
                   .WithMany()
                   .HasForeignKey(c => c.PriorityBookingId)
                   .OnDelete(DeleteBehavior.Restrict);
            // Restrict: Tương tự như trên
        }
    }
}
