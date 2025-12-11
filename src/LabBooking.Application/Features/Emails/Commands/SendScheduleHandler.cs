using LabBooking.Application.Services;

namespace LabBooking.Application.Features.Emails.Commands;

public class SendScheduleHandler : IRequestHandler<SendScheduleCommand, string>
{
    private readonly IExcelService _excelService;
    private readonly IEmailService _emailService;
    private readonly IBackgroundJobService _jobService;

    public SendScheduleHandler(
        IExcelService excelService,
        IEmailService emailService,
        IBackgroundJobService jobService)
    {
        _excelService = excelService;
        _emailService = emailService;
        _jobService = jobService;
    }

    public async Task<string> Handle(SendScheduleCommand request, CancellationToken cancellationToken)
    {
        // 1. Đọc danh sách sinh viên (Logic tách biệt)
        var students = _excelService.ReadStudents(request.StudentListStream);

        if (students == null || !students.Any()) return "File danh sách rỗng!";

        // 2. Đẩy job vào hàng đợi (Không gửi ngay lập tức để tránh treo server)
        foreach (var student in students)
        {
            // Enqueue: Lưu job vào Database, Worker sẽ xử lý sau
            _jobService.Enqueue(() =>
                _emailService.SendEmailAsync(
                    student.Email,
                    "Thông báo Thời Khóa Biểu Mới",
                    $"<h3>Xin chào {student.FullName}</h3><p>Gửi bạn lịch học mới.</p>",
                    request.Attachment
                ));
        }

        return $"Đã lên lịch gửi thành công cho {students.Count} sinh viên.";
    }
}
