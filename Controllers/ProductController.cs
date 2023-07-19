using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace SampleRedis.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
     private readonly ILogger<ProductController> _logger;
     private readonly IDistributedCache _cache;

    public ProductController(ILogger<ProductController> logger, IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
    }


    [HttpGet(Name = "GetProducts")]
    public async Task<IEnumerable<Product>> Get()
    {
        var cacheKey = "Products";
        var products = new List<Product>();

        var json = await _cache.GetStringAsync(cacheKey);
        if(json != null)
        {
            products = JsonSerializer.Deserialize<List<Product>>(json);
        }
        return products;
    }

    [HttpPost(Name = "PostProducts")]
    public async Task<bool> Post()
    {
        var cacheKey = "Products";
        var products = new List<Product>();

        var json = await _cache.GetStringAsync(cacheKey);
        if(json != null)
        {
            products = JsonSerializer.Deserialize<List<Product>>(json);
        }
        else {
            var product = new Product { Id = 1, Description = "Product 001", CreateDate = DateTime.Now };
            products.Add(product); 
            product = new Product { Id = 2, Description = "Product 002", CreateDate = DateTime.Now };
            products.Add(product); 
            product = new Product { Id = 3, Description = "Product 003", CreateDate = DateTime.Now };
            products.Add(product); 

            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(20))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

            json = JsonSerializer.Serialize<List<Product>>(products);
            await _cache.SetStringAsync(cacheKey, json, options);
        }
        return true;
    }


     [HttpDelete(Name = "DeleteProducts")]
    public async Task<bool> Delete()
    {
        var cacheKey = "Products";
        await _cache.RemoveAsync(cacheKey);
        return true;
    }
}