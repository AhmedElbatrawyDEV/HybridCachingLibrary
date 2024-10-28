using EasyCaching.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HybridCachingLibrary;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHybridCaching(this IServiceCollection services, IConfiguration configuration) {
        // Bind HybridCacheOptions from the configuration
        services.Configure<HybridCacheOptions>(configuration.GetSection("HybridCacheOptions"));

        services.AddEasyCaching(options =>
        {
            // Retrieve options instance
            var cacheOptions = configuration.GetSection("HybridCacheOptions").Get<HybridCacheOptions>();

            // Use in-memory caching with JSON serializer
            options.UseInMemory(cacheOptions.InMemoryCacheInstanceName)
                .WithJson();

            // Use Redis caching with JSON serializer
            options.UseRedis(redisOptions =>
            {
                redisOptions.DBConfig.Endpoints.Add(new ServerEndPoint(cacheOptions.RedisHost, cacheOptions.RedisPort));
                redisOptions.DBConfig.Database = 0;
                redisOptions.SerializerName = "json"; // Set the serializer name
            }, cacheOptions.RedisInstanceName);

            // Configure hybrid caching
            options.UseHybrid(hybridOptions =>
                {
                    hybridOptions.TopicName = "hybrid-cache-topic";
                    hybridOptions.LocalCacheProviderName = cacheOptions.InMemoryCacheInstanceName;
                    hybridOptions.DistributedCacheProviderName = cacheOptions.RedisInstanceName;
                })
                .WithRedisBus(busOptions => {
                    busOptions.Endpoints.Add(new ServerEndPoint(cacheOptions.RedisHost, cacheOptions.RedisPort));
                    busOptions.SerializerName = "json"; // Set the serializer name for the bus
                });
        });

        services.AddTransient<HybridCacheService>();

        return services;
    }

}