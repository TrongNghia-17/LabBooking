namespace LabBooking.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string? Major { get; set; } // Chỉ cho sinh viên
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    // Quan hệ
    public ICollection<Notification>? Notifications { get; set; }
    public ICollection<Booking>? Bookings { get; set; }
    public ICollection<Incident>? Incidents { get; set; } // chỉ cho bảo vệ
    public ICollection<UserDevice>? UserDevices { get; set; }
}
