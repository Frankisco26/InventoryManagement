using InventoryManagement.Data.Dtos;
using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Data.Services
{
    public interface IProductService
    {
        Task<ProductDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllAsync(string? category = null, int? lowStockThreshold = null);
        Task<ProductDto> CreateAsync(CreateProductDto dto, string userId);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ProductExistsAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return null;

            return MapToDto(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(string? category = null, int? lowStockThreshold = null)
        {
            var query = _context.Products.AsQueryable();

            // Filtrar por categoría si se proporciona
            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.Category.ToLower().Contains(category.ToLower()));

            // Filtrar por stock bajo si se proporciona umbral
            if (lowStockThreshold.HasValue)
                query = query.Where(p => p.QuantityInStock < lowStockThreshold.Value);

            var products = await query.OrderBy(p => p.Name).ToListAsync();

            return products.Select(MapToDto);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto, string userId)
        {
            // Validar que el SKU sea único
            var skuExists = await _context.Products
                .AnyAsync(p => p.SKU.ToLower() == dto.SKU.ToLower());

            if (skuExists)
                throw new InvalidOperationException($"Ya existe un producto con el SKU '{dto.SKU}'");

            var product = new Product
            {
                Name = dto.Name,
                SKU = dto.SKU.ToUpper(),
                Category = dto.Category,
                QuantityInStock = dto.QuantityInStock,
                MinimumStock = dto.MinimumStock,
                UnitPrice = dto.UnitPrice,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Producto creado: {product.Id} - {product.Name}");

            return MapToDto(product);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, string userId)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");

            product.Name = dto.Name;
            product.Category = dto.Category;
            product.MinimumStock = dto.MinimumStock;
            product.UnitPrice = dto.UnitPrice;
            product.UpdatedAt = DateTime.Now;
            product.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Producto actualizado: {product.Id} - {product.Name}");

            return MapToDto(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Producto eliminado: {id}");

            return true;
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Category = product.Category,
                QuantityInStock = product.QuantityInStock,
                MinimumStock = product.MinimumStock,
                UnitPrice = product.UnitPrice,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                CreatedBy = product.CreatedBy
            };
        }
    }
}
