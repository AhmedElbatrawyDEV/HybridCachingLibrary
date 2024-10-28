using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HybridCachingLibrary;

public class HybridCacheService(IHybridCachingProvider _hybridCacheProvider, ILogger<HybridCacheService> _logger)
{
    public async Task<T?> GetOrAddAsync<T>(string key, Func<Task<T>> dataRetrievalFunc, TimeSpan? expiration = null)
    {
        var cacheResult = await _hybridCacheProvider.GetAsync<T>(key);
        if (cacheResult.HasValue)
        {
            _logger.LogInformation($"Cache hit for key: {key}");
            return cacheResult.Value;
        }

        _logger.LogInformation($"Cache miss for key: {key}. Retrieving data...");
        var data = await dataRetrievalFunc();

        await _hybridCacheProvider.SetAsync(key, data, expiration ?? TimeSpan.FromMinutes(5));
        return data;
    }

    public async Task RemoveAsync(string key)
    {
        await _hybridCacheProvider.RemoveAsync(key);
    }
}