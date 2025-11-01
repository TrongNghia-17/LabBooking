namespace LabBooking.Domain.Entities;

public class UserDevice
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string PushToken { get; set; } = default!;
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public User? User { get; set; }

}
