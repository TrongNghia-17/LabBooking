namespace LabBooking.Infrastructure.Repositories;

internal class RefreshTokenRepository(LabBookingDbContext dbContext) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        await dbContext.RefreshTokens.AddAsync(token, cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
        var refreshTokens = await dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        return refreshTokens;
    }

    /// <summary>
    /// Revoke (invalidate) all valid refresh tokens of a user.
    /// Use EF Core 7+ ExecuteUpdateAsync for best performance.
    /// </summary>
    public async Task RevokeAllTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        // Find all tokens of this user that are STILL VALID (not revoked AND not expired)
        // and update them immediately in the DB.
        await dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.Revoked == null && rt.Expires > now)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(rt => rt.Revoked, now),
            cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int check = await dbContext.SaveChangesAsync(cancellationToken);
        return check;
    }

    public void Update(RefreshToken token)
    {
        dbContext.RefreshTokens.Update(token);
    }
}
