namespace LabBooking.Infrastructure.Implements.Authentication.Users;

public class UserFactory : IUserFactory
{
    public User CreateUserFromGooglePayload(GoogleJsonWebSignature.Payload payload)
    {
        string safeUserName = payload.Email.Split('@')[0];

        if (payload.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
        {
            return new User
            {
                Email = payload.Email,
                FullName = payload.GivenName,
                UserName = safeUserName,
                Major = "",
                EmailConfirmed = true,
                RegistrationDate = DateTime.UtcNow
            };
        }

        string email = payload.Email;
        string givenName = payload.GivenName ?? "";
        string majorCode = ParseMajorFromEmail(email);

        return new User
        {
            Email = email,
            UserName = safeUserName,
            FullName = givenName,
            Major = majorCode,
            EmailConfirmed = true,
            RegistrationDate = DateTime.UtcNow
        };
    }

    private string ParseMajorFromEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) return string.Empty;

        var parts = email.Split('@');
        if (parts.Length < 2) return string.Empty;

        var usernamePart = parts[0];

        var match = Regex.Match(usernamePart, @"([a-zA-Z]{2})\d+$");

        return match.Success ? match.Groups[1].Value.ToUpper() : string.Empty;
    }
}