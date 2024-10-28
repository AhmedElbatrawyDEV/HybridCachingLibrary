using HybridCachingLibrary;
using Microsoft.AspNetCore.Mvc;

namespace HybridCachingWebApiSample.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase {
        private readonly HybridCacheService _hybridCacheService;

        public ProductsController(HybridCacheService hybridCacheService) {
            _hybridCacheService = hybridCacheService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id) {
            var cacheKey = $"product_{id}";
            var product = await _hybridCacheService.GetOrAddAsync(
                cacheKey, () => Task.FromResult(new Product { Id = id, Name = $"Product {id}", Description = "Sample product" }),
                TimeSpan.FromMinutes(5)
            );
            return Ok(product);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id) {
            var cacheKey = $"product_{id}";
            await _hybridCacheService.RemoveAsync(cacheKey);
            return Ok();
        }
    }

}
