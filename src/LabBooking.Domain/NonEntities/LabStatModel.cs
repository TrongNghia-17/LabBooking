namespace LabBooking.Domain.NonEntities;

public class LabStatModel
{
    public Guid LabId { get; set; }
    public string LabName { get; set; } = string.Empty;
    public int Month { get; set; }
    public bool IsMaintenance { get; set; }
    public Guid BookingId { get; set; } // Dùng để đếm Distinct
}
