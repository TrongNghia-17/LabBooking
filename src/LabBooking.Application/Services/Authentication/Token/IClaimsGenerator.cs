namespace LabBooking.Application.Services.Authentication.Token;

public interface IClaimsGenerator
{
    /// <summary>
    /// Tạo danh sách các claims (bao gồm roles) cho một user
    /// </summary>
    Task<List<Claim>> GenerateClaimsAsync(User user);
}
