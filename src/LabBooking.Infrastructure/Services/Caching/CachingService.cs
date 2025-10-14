namespace LabBooking.Infrastructure.Services.Caching;

internal class CachingService(IDistributedCache cache) : ICachingService
{
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

    public async Task<bool> ExistsAsync(string cacheKey)
    {
        var cachedData = await cache.GetAsync(cacheKey);
        return cachedData != null;
    }

    public async Task<T> GetOrSetAsync<T>(string cacheKey,
        Func<Task<T>> factory,
        TimeSpan? expiration = default)
    {
        var cachedData = await cache.GetAsync(cacheKey);

        try
        {
            if (cachedData != null)
            {
                var jsonString = Encoding.UTF8.GetString(cachedData);
                var result = JsonSerializer.Deserialize<T>(jsonString)
                     ?? throw new InvalidOperationException("Deserialized object is null");
                return result;
            }
        }
        catch (JsonException)
        {
            await cache.RemoveAsync(cacheKey);
        }

        var data = await factory();

        var jsonToCache = JsonSerializer.Serialize(data);
        var content = Encoding.UTF8.GetBytes(jsonToCache);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = expiration ?? _defaultExpiration
        };

        await cache.SetAsync(cacheKey, content, cacheOptions);

        return data;
    }

    public async Task RemoveAsync(string cacheKey)
    {
        await cache.RemoveAsync(cacheKey);
    }
}
