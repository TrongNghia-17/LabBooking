using LabBooking.Application.Services;

namespace LabBooking.Application.Features.Emails.Commands;

public class SendScheduleHandler : IRequestHandler<SendScheduleCommand, string>
{
    private readonly IExcelService _excelService;
    private readonly IEmailService _emailService;
    private readonly IBackgroundJobService _jobService;
    private readonly IEmailTemplateService _templateService;

    public SendScheduleHandler(
        IExcelService excelService,
        IEmailService emailService,
        IBackgroundJobService jobService,
        IEmailTemplateService templateService)
    {
        _excelService = excelService;
        _emailService = emailService;
        _jobService = jobService;
        _templateService = templateService;
    }

    public async Task<string> Handle(SendScheduleCommand request, CancellationToken cancellationToken)
    {
        // 1. Đọc danh sách sinh viên (Logic tách biệt)
        var students = _excelService.ReadStudents(request.StudentListStream);

        if (students == null || !students.Any()) return "File danh sách rỗng!";

        // 2. Đẩy job vào hàng đợi (Không gửi ngay lập tức để tránh treo server)
        foreach (var student in students)
        {
            // -- BƯỚC QUAN TRỌNG: TẠO NỘI DUNG DYNAMIC --
            // Thay thế {{FullName}} thành tên thật
            string personalizedBody = _templateService.GenerateContent(request.BodyTemplate, student);
            // -------------------------------------------

            // 3. Enqueue
            _jobService.Enqueue(() =>
                _emailService.SendEmailAsync(
                    student.Email,
                    request.Subject,     // Dùng subject từ Admin nhập
                    personalizedBody,    // Dùng body đã được cá nhân hóa
                    request.Attachment   // File đính kèm (nếu có)
                ));
        }

        return $"Đã lên lịch gửi thành công cho {students.Count} sinh viên.";
    }
}
