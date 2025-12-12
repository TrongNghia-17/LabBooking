using LabBooking.Application.Interfaces.Infrastructure;

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
        // 1. Đọc danh sách từ Excel
        var students = _excelService.ReadStudents(request.StudentListStream);

        if (students == null || !students.Any()) return "File danh sách rỗng!";

        // 2. Cấu hình độ trễ (Delay) để không bị chặn kết nối
        int delaySeconds = 5; // Cứ 5 giây gửi 1 mail

        for (int i = 0; i < students.Count; i++)
        {
            var student = students[i];

            // Tạo nội dung riêng cho từng người
            string personalizedBody = _templateService.GenerateContent(request.BodyTemplate, student);

            // Tính thời gian gửi: Người thứ i sẽ được gửi sau (i * 5) giây
            // VD: Người 0: 0s, Người 1: 5s, Người 2: 10s...
            var scheduleTime = TimeSpan.FromSeconds(i * delaySeconds);

            // 3. Dùng SCHEDULE thay vì Enqueue
            _jobService.Schedule(() =>
                _emailService.SendEmailAsync(
                    student.Email,
                    request.Subject,
                    personalizedBody,
                    request.Attachment
                ), scheduleTime);
        }

        return $"Đã lên lịch thành công cho {students.Count} sinh viên. Mail sẽ được gửi lần lượt trong vòng {students.Count * delaySeconds} giây.";
    }
}