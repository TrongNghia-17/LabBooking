using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.SecurityGuards.Commands.UpdateSecurityGuard
{
    public record UpdateSecurityGuardCommand : IRequest<Unit>
    {
        [JsonIgnore]
        public Guid Id { get; set; } // Lấy từ URL

        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        // Thêm FullName nếu cần
    }
}
