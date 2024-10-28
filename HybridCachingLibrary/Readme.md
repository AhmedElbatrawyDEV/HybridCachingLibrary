
# Hybrid Caching Library

A brief description of what this project does and who it's for

HybridCachingLibrary
HybridCachingLibrary is a .NET library that provides caching capabilities using both in-memory and Redis cache. This approach combines the speed of in-memory caching with the scalability of Redis, making it ideal for distributed systems.
## Features

- **Hybrid Caching**: Leverages both in-memory and Redis caching for optimized performance and scalability.
- **Easy Integration**: Simple configuration through `appsettings.json`.
- **Configurable Cache Expiration**: Control cache expiration at the application level.

## Requirements
To use HybridCachingLibrary, the following NuGet packages should be installed in your project:

EasyCaching.Core
EasyCaching.InMemory
EasyCaching.Redis
EasyCaching.Bus.Redis
Microsoft.Extensions.DependencyInjection
Microsoft.Extensions.Options.ConfigurationExtensions
These packages are necessary to handle caching operations and configure options from appsettings.json.

## Setup Guide

### Step 1: Configure `appsettings.json`

Add a `HybridCacheOptions` section to `appsettings.json` in your project:

```json
{
  "HybridCacheOptions": {
    "RedisHost": "localhost",
    "RedisPort": 6379,
    "RedisInstanceName": "redisCache",
    "InMemoryCacheInstanceName": "inMemoryCache",
    "CacheExpirationMinutes": 5
  }
}
```
## Explanation of Settings:

RedisHost: The hostname or IP of the Redis server (e.g., "localhost" or an IP address).
RedisPort: The port for the Redis server, typically 6379.
RedisInstanceName: A name for the Redis instance within the cache.
InMemoryCacheInstanceName: A name for the in-memory cache instance.
CacheExpirationMinutes: How long items should remain in the cache (in minutes).
### Step 2: Configure Caching in Program.cs
Configure Caching in Your Application
In the project where you want to use the library, go to Program.cs or Startup.cs, and add the following configuration:

```csharp
using HybridCachingLibrary;

var builder = WebApplication.CreateBuilder(args);

// Register the HybridCachingLibrary, linking it to configuration from appsettings.json
builder.Services.AddHybridCaching(builder.Configuration);

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();
```
This code registers the hybrid cache so it reads options from appsettings.json and sets up both in-memory and Redis caches.

Example Usage
Here’s how you might use the HybridCacheService in a controller:

```csharp
using HybridCachingLibrary;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly HybridCacheService _hybridCacheService;

    public ProductsController(HybridCacheService hybridCacheService)
    {
        _hybridCacheService = hybridCacheService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var cacheKey = $"product_{id}";

        // Use the cache service to get or add a product in cache
        var product = await _hybridCacheService.GetOrAddAsync(
            cacheKey,
            async () => new Product { Id = id, Name = $"Product {id}", Description = "Sample product" },
            TimeSpan.FromMinutes(5)
        );

        return Ok(product);
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
```
## License
This library is licensed under the MIT License.