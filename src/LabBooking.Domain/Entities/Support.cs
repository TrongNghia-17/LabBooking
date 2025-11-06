namespace LabBooking.Domain.Entities;

public class Support
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string Answer { get; set; } = default!;
    public Guid CreatedById { get; set; }
}
