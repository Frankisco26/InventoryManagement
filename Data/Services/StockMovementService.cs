
using InventoryManagement.Data.Dtos;
using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Data.Services
{
    public interface IStockMovementService
    {
        Task<StockMovementDto> CreateAsync(int productId, CreateStockMovementDto dto, string userId);
        Task<IEnumerable<StockMovementDto>> GetByProductIdAsync(int productId);
        Task<StockMovementDto?> GetByIdAsync(int id);
    }

    public class StockMovementService : IStockMovementService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;
        private readonly ILogger<StockMovementService> _logger;

        public StockMovementService(
            ApplicationDbContext context, 
            IProductService productService,
            ILogger<StockMovementService> logger)
        {
            _context = context;
            _productService = productService;
            _logger = logger;
        }

        public async Task<StockMovementDto> CreateAsync(int productId, CreateStockMovementDto dto, string userId)
        {
            // Validar que el producto existe
            if (!await _productService.ProductExistsAsync(productId))
                throw new KeyNotFoundException($"Producto con ID {productId} no encontrado");

            // Obtener el producto
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Producto con ID {productId} no encontrado");

            // Validar stock negativo para movimientos de salida
            if (dto.Type == StockMovementType.Outbound && product.QuantityInStock < dto.Quantity)
                throw new InvalidOperationException(
                    $"Stock insuficiente. Disponible: {product.QuantityInStock}, Solicitado: {dto.Quantity}");

            // Crear movimiento de stock
            var movement = new StockMovement
            {
                ProductId = productId,
                Type = dto.Type,
                Quantity = dto.Quantity,
                Reason = dto.Reason,
                Timestamp = DateTime.Now,
                CreatedBy = userId
            };

            // Actualizar stock del producto
            if (dto.Type == StockMovementType.Inbound)
                product.QuantityInStock += dto.Quantity;
            else if (dto.Type == StockMovementType.Outbound)
                product.QuantityInStock -= dto.Quantity;

            product.UpdatedAt = DateTime.Now;
            product.UpdatedBy = userId;

            _context.StockMovements.Add(movement);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                $"Movimiento de stock creado: Producto {productId}, Tipo: {dto.Type}, Cantidad: {dto.Quantity}");

            return MapToDto(movement);
        }

        public async Task<IEnumerable<StockMovementDto>> GetByProductIdAsync(int productId)
        {
            var movements = await _context.StockMovements
                .Where(sm => sm.ProductId == productId)
                .OrderByDescending(sm => sm.Timestamp)
                .ToListAsync();

            return movements.Select(MapToDto);
        }

        public async Task<StockMovementDto?> GetByIdAsync(int id)
        {
            var movement = await _context.StockMovements
                .FirstOrDefaultAsync(sm => sm.Id == id);

            if (movement == null)
                return null;

            return MapToDto(movement);
        }

        private StockMovementDto MapToDto(StockMovement movement)
        {
            return new StockMovementDto
            {
                Id = movement.Id,
                ProductId = movement.ProductId,
                Type = movement.Type.ToString(),
                Quantity = movement.Quantity,
                Reason = movement.Reason,
                Timestamp = movement.Timestamp,
                CreatedBy = movement.CreatedBy
            };
        }
    }
}
