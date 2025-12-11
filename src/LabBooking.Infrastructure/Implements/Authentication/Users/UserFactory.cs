namespace LabBooking.Infrastructure.Implements.Authentication.Users;

public class UserFactory : IUserFactory
{
    public User CreateUserFromGooglePayload(GoogleJsonWebSignature.Payload payload)
    {
        // Cách lấy UserName an toàn nhất: Lấy phần trước @ của email
        // Ví dụ: "nghia.admin@gmail.com" -> "nghia.admin" (Không dấu cách, không ký tự lạ)
        string safeUserName = payload.Email.Split('@')[0];

        // --- TRƯỜNG HỢP 1: GMAIL (Manager) ---
        if (payload.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
        {
            return new User
            {
                Email = payload.Email,
                UserName = safeUserName, // FIX: Dùng safeUserName thay vì payload.Name
                Major = "Manager",
                EmailConfirmed = true,
                RegistrationDate = DateTime.UtcNow
            };
        }

        // --- TRƯỜNG HỢP 2: FPT MAIL (Student/Lecturer) ---
        string email = payload.Email;

        // Fix null safety: Nếu Google không trả về tên thì gán rỗng để tránh lỗi .Trim()
        string familyName = payload.FamilyName ?? "";
        string givenName = payload.GivenName ?? "";

        // 1. Logic lấy Major (SE, AI,...) từ Email
        string majorCode = ParseMajorFromEmail(email); // "SE"

        // 2. Logic lấy Class/Campus (K17 HCM)
        string classInfo = familyName.Trim('(', ')', ' '); // "K17 HCM"

        // 3. Logic lấy UserName sạch
        // FIX: Không dùng givenName.Trim() nữa vì nó chứa dấu cách/tiếng Việt
        string userName = safeUserName;

        // 4. Kết hợp Major (Xử lý trường hợp không có majorCode để chuỗi đẹp hơn)
        string finalMajor = string.IsNullOrEmpty(majorCode)
                            ? classInfo
                            : $"{majorCode} {classInfo}";

        // 5. Tạo User
        return new User
        {
            Email = email,
            UserName = userName,
            Major = finalMajor.Trim(), // Trim lần cuối cho sạch
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

        // Dùng Regex tìm 2 chữ cái đứng ngay trước 1 dãy số (Format sinh viên SE123456)
        var match = Regex.Match(usernamePart, @"([A-Za-z]{2})\d+$");

        if (match.Success)
        {
            return match.Groups[1].Value.ToUpper(); // "SE"
        }

        return string.Empty;
    }
}