namespace LabBooking.Application.Features.Auths.Dtos;

public class UserDto
{
    public Guid Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
}
