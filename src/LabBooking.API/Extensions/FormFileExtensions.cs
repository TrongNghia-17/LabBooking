using LabBooking.Application.Features.Emails.Dtos;

namespace LabBooking.API.Extensions;

public static class FormFileExtensions
{
    public static async Task<EmailAttachmentDto?> ToAttachmentDtoAsync(this IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        return new EmailAttachmentDto
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileContent = ms.ToArray()
        };
    }
}