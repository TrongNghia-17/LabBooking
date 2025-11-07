using System.Text.Json.Serialization;

namespace LabBooking.Application.Features.Users.Commands.AssignRole;

public record AssignRoleToUserCommand : IRequest<Unit>
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    public string RoleName { get; set; } = default!;
}
