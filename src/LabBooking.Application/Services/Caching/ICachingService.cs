namespace LabBooking.Application.Services.Caching;

/// <summary>
/// Interface for caching operations
/// </summary>
public interface ICachingService
{
    /// <summary>
    /// Get cached data or execute factory function and cache the result
    /// </summary>
    /// <typeparam name="T">Type of data to cache</typeparam>
    /// <param name="cacheKey">Unique cache key</param>
    /// <param name="factory">Function to execute if cache miss</param>
    /// <param name="expiration">Cache expiration time</param>
    /// <returns>Cached or newly computed data</returns>
    Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? expiration = default);

    /// <summary>
    /// Remove item from cache
    /// </summary>
    /// <param name="cacheKey">Cache key to remove</param>
    Task RemoveAsync(string cacheKey);

    /// <summary>
    /// Check if key exists in cache
    /// </summary>
    /// <param name="cacheKey">Cache key to check</param>
    Task<bool> ExistsAsync(string cacheKey);
}
