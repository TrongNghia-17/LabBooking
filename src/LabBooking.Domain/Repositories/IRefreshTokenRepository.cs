namespace LabBooking.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken);
    void Update(RefreshToken token);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke (invalidate) all valid refresh tokens of a user.
    /// Often used when detecting attacks (replay attacks).
    /// </summary>
    /// <param name="userId">User ID</param>
    Task RevokeAllTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
