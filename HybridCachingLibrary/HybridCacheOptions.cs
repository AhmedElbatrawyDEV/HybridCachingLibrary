namespace HybridCachingLibrary;

public class HybridCacheOptions
{
    public string RedisHost { get; set; }
    public int RedisPort { get; set; }
    public string RedisInstanceName { get; set; }
    public string InMemoryCacheInstanceName { get; set; }
    public int CacheExpirationMinutes { get; set; }
}