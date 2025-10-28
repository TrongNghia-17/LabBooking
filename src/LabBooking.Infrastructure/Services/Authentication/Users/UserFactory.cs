namespace LabBooking.Infrastructure.Services.Authentication.Users;

public class UserFactory : IUserFactory
{
    public User CreateUserFromGooglePayload(GoogleJsonWebSignature.Payload payload)
    {
        string email = payload.Email;
        string familyName = payload.FamilyName; // "(K17 HCM)"
        string givenName = payload.GivenName; // "Huynh Trong Nghia"

        // 1. Logic lấy Major (SE, AI,...) từ Email
        string majorCode = ParseMajorFromEmail(email); // "SE"

        // 2. Logic lấy Class/Campus (K17 HCM)
        string classInfo = familyName.Trim('(', ')', ' '); // "K17 HCM"

        // 3. Logic lấy UserName sạch
        string userName = givenName.Trim(); // "Huynh Trong Nghia"

        // 4. Kết hợp Major
        string finalMajor = $"{majorCode} {classInfo}"; // "SE K17 HCM"

        // 5. Tạo User
        return new User
        {
            Email = email,
            UserName = userName,
            Major = finalMajor,
            EmailConfirmed = true,
            RegistrationDate = DateTime.UtcNow
        };
    }

    private string ParseMajorFromEmail(string email)
    {
        // "nghiahtse172725@fpt.edu.vn"
        var username = email.Split('@')[0]; // "nghiahtse172725"

        // Dùng Regex tìm 2 chữ cái đứng ngay trước 1 dãy số
        var match = Regex.Match(username, @"([A-Za-z]{2})\d+$");

        if (match.Success)
        {
            return match.Groups[1].Value.ToUpper(); // "SE"
        }

        return string.Empty;
    }
}
