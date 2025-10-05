namespace LabBooking.Domain.Entities;

public class Project
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();

    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid OwnerId { get; set; }
    [ForeignKey(nameof(OwnerId))]
    public ProjectType ProjectType { get; set; } = ProjectType.Other;
    public User? Owner { get; set; } // Lecturer hoặc Student đều được

    public ICollection<User>? Members { get; set; } // Nhóm sinh viên hoặc giảng viên được mời
}

public enum ProjectType
{
    Presentation = 0,
    TalkShow = 1,
    Research = 2,
    Competition = 3,
    Workshop = 4,
    Other = 5
}
