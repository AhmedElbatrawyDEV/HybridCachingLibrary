using EasyCaching.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HybridCachingLibrary;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHybridCaching(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind HybridCacheOptions from the consuming project's configuration
        services.Configure<HybridCacheOptions>(configuration.GetSection("HybridCacheOptions"));

        services.AddEasyCaching(options =>
        {
            // Retrieve options instance using dependency injection
            var cacheOptions = configuration.GetSection("HybridCacheOptions").Get<HybridCacheOptions>();

            options.UseInMemory(cacheOptions.InMemoryCacheInstanceName);

            options.UseRedis(redisOptions =>
            {
                redisOptions.DBConfig.Endpoints.Add(new ServerEndPoint(cacheOptions.RedisHost, cacheOptions.RedisPort));
                redisOptions.DBConfig.Database = 0;
            }, cacheOptions.RedisInstanceName);

            options.UseHybrid(hybridOptions =>
                {
                    hybridOptions.TopicName = "hybrid-cache-topic";
                    hybridOptions.LocalCacheProviderName = cacheOptions.InMemoryCacheInstanceName;
                    hybridOptions.DistributedCacheProviderName = cacheOptions.RedisInstanceName;
                })
                .WithRedisBus(busOptions =>
                {
                    busOptions.Endpoints.Add(new ServerEndPoint(cacheOptions.RedisHost, cacheOptions.RedisPort));
                });
        });

        services.AddTransient<HybridCacheService>();

        return services;
    }
}