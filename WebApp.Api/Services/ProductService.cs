using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Data;
using WebApp.Api.DTOs;
using WebApp.Api.Models;
using WebApp.Api.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace WebApp.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly IDistributedCache _cache;

        private const string ProductsCacheKey = "products:list";
        private const string ProductByIdCacheKeyPrefix = "products:id";

        public object ProductId { get; private set; }

        public ProductService(AppDbContext context, IMapper mapper, ILogger<ProductService> logger, IDistributedCache cache)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<(IEnumerable<ProductDto> Data, int TotalCount)> GetAllProductsAsync(ProductQueryParameters query)
        {
            var cacheKey = $"{ProductsCacheKey}:page={query.Page}:size={query.PageSize}:search={query.Search}:min={query.MinPrice}:max={query.MaxPrice}:sort={query.SortBy}:desc={query.Descending}";

            var cacedData = await _cache.GetStringAsync(cacheKey);

            if(cacedData != null)
            {
                _logger.LogInformation("Products list loaded from redis cache. key : {cacheKey}", cacheKey);
                var cachedResult = JsonSerializer.Deserialize<ProductListResponse>(cacedData);

                return (cachedResult!.Data, cachedResult.TotalCount);
            }
            _logger.LogInformation($"Products list not found in redis cache. loading from database. key : {cacheKey}", cacheKey);
            var productsQuery = _context.Products.AsQueryable();
            if(!string.IsNullOrWhiteSpace(query.Search))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(query.Search));
            }
            
            if(query.MinPrice.HasValue) 
            {
                productsQuery = productsQuery.Where(p => p.Price >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= query.MaxPrice.Value);
            }

            var totalCount = await productsQuery.CountAsync();

            productsQuery = query.SortBy?.ToLower() switch
            {
                "name" => query.Descending ? productsQuery.OrderByDescending(p => p.Name) : productsQuery.OrderBy(p => p.Name),
                "price" => query.Descending ? productsQuery.OrderByDescending(p => p.Price) : productsQuery.OrderBy(p => p.Price),
                _ => productsQuery.OrderBy(p => p.Id)
            };

            var products = await productsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            _logger.LogInformation("Retrieving products with pagination.");

            var data = _mapper.Map<IEnumerable<ProductDto>>(products);
            var result = (data, totalCount);
            var serialized = JsonSerializer.Serialize(result);
            await _cache.SetStringAsync(
                cacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3),
                    SlidingExpiration = TimeSpan.FromMinutes(1),
                });
            _logger.LogInformation($"Products saved to redis cache.key : {cacheKey}", cacheKey);
            return (result.data, result.totalCount);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id) 
        {
            var cacheKey = $"product:{id}";

            var cacedData = await _cache.GetStringAsync(cacheKey);
            if(cacedData != null)
            {
                _logger.LogInformation("product with Id {ProductId} loaded from cache", id);

                return JsonSerializer.Deserialize<ProductDto>(cacedData);
            }

            _logger.LogInformation("Product with ID {ProductId} not found redis in cache.Loadingfrom DB.", id);

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return null;
            }

            var result = _mapper.Map<ProductDto>(product);
            var serialized = JsonSerializer.Serialize(result);
           await _cache.SetStringAsync(
                cacheKey, 
                serialized, 
                new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            });

            return result;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto) 
        {
            _logger.LogInformation("Creating product with name: {ProductName}", dto.Name);
            var product = _mapper.Map<Product>(dto);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            RemoveProductCaches();
            _logger.LogInformation("Product created seccessfuly with ID: {ProductID}", product.Id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, CreateProductDto dto) 
        {
            var product = await _context.Products.FindAsync(id);
            if(product == null)
            {
                _logger.LogWarning("Product update faild. product widt ID: {ProductId} was not found.", id);
                return null;
            }
            _mapper.Map(dto, product);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"product{id}");
            return _mapper.Map<ProductDto>(product);
        }

       public async Task<bool> DeleteProductAsync(int id) 
        {
            var product = await _context.Products.FindAsync(id);
            if(product == null)
            {
                _logger.LogWarning("Product delete faild. product widt ID: {ProductId} was not found.", id);
                return false;
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"product{id}");
            return true;
        }

        private void RemoveProductCaches (int? productId = null)
        {
            if (productId.HasValue)
            {
                _cache.Remove($"{ProductByIdCacheKeyPrefix}{productId.Value}");
            }

            _logger.LogInformation($"product cache invaleted : {ProductId}");
        }
    }
}
