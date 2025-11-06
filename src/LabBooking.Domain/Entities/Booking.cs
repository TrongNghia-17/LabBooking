namespace LabBooking.Domain.Entities;

public enum BookingStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled
}

public enum BookingType
{
    Teaching,   // Lịch dạy học
    Project     // Lịch dự án
}

public enum BookingPriority
{
    UniversityEvent = 1,
    LecturerTeaching = 2,
    LecturerResearch = 3,
    StudentGraduationProject = 4,
    StudentClubEvent = 5,
    StudentNormal = 6
}

public class Booking
{
    public Guid Id { get; set; } = (Guid)Uuid7.NewUuid7();
    public Guid LabRoomId { get; set; }
    [ForeignKey(nameof(LabRoomId))]
    public LabRoom? LabRoom { get; set; }
    public Guid CreatedById { get; set; }
    [ForeignKey(nameof(CreatedById))]
    public User? CreatedBy { get; set; }
    public string? Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public BookingStatus? Status { get; set; } = BookingStatus.Pending;
    public bool? IsPublic { get; set; }
    public bool? IsMajorOnly { get; set; }
    public BookingType? Type { get; set; }

    public BookingPriority? Priority { get; set; } = BookingPriority.StudentNormal;


    // Nếu là lịch dạy học → lưu tên môn học
    // danh sách các môn học có sẵn (tạo bởi admin)
    public Guid? CourseId { get; set; }
    [ForeignKey(nameof(CourseId))]
    public Course? Course { get; set; }

    // Nếu là lịch dự án → liên kết tới Project
    public Guid? ProjectId { get; set; }
    [ForeignKey(nameof(ProjectId))]
    public Project? Project { get; set; }
    public int? NumberOfParticipants { get; set; }
    public ICollection<BookingSlot>? Slots { get; set; }
    public ICollection<BookingParticipant>? Participants { get; set; }
}
