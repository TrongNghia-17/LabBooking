namespace LabBooking.Application.Common.Dtos;

public class EmailAttachmentDto
{
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
    public string ContentType { get; set; }
}