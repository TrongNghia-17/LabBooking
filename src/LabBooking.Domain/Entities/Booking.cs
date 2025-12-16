namespace LabBooking.Domain.Entities;

public enum BookingStatus
{
    Pending, //0
    Approved, //1
    Rejected, //2
    Cancelled //3
}

public enum BookingType
{
    Teaching,  //0 // Lịch dạy học
    Project,    //1 // Lịch dự án
    UniversityEvent //2 // Lịch sự kiện trường
}

public enum BookingPriority
{
    Maintenance = 0,
    UniversityEvent = 1,
    Standard = 2
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

    public Guid? ApprovedById { get; set; }

    public DateTime? ApprovedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public BookingType? Type { get; set; }

    public BookingPriority? Priority { get; set; } = BookingPriority.Standard;


    // Nếu là lịch dạy học → lưu tên môn học
    // danh sách các môn học có sẵn (tạo bởi admin)
    public Guid? CourseId { get; set; }
    [ForeignKey(nameof(CourseId))]
    public Course? Course { get; set; }

    // Nếu là lịch dự án → liên kết tới Project
    public Guid? ProjectId { get; set; }
    [ForeignKey(nameof(ProjectId))]
    public Project? Project { get; set; }

    // Nếu là lịch ưu tiên -> liên kết tới lý do
    public Guid? BookingPriorityDetailId { get; set; }
    [ForeignKey(nameof(BookingPriorityDetailId))]
    public BookingPriorityDetail? BookingPriorityDetail { get; set; }
    public int? NumberOfParticipants { get; set; }
    public ICollection<BookingSlot>? Slots { get; set; }
    public ICollection<ExternalEquipment>? ExternalEquipments { get; set; }
    public ICollection<OutSideGuest>? OutSideGuests { get; set; }
    public string? PendingSlotsJson { get; set; }
    public string? QrCodeString { get; set; }
}
