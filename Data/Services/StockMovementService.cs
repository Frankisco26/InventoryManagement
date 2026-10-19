using InventoryManagement.Data.Base;
using InventoryManagement.Data.Dtos;
using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Data.Services
{
    /// <summary>
    /// Todas las operaciones se hacen dentro de la empresa indicada (multiempresa).
    /// </summary>
    public interface IStockMovementService
    {
        Task<StockMovementDto> CreateAsync(int empresaId, int productId, CreateStockMovementDto dto, string userId);
        Task<IEnumerable<StockMovementDto>> GetByProductIdAsync(int empresaId, int productId);
        Task<StockMovementDto?> GetByIdAsync(int empresaId, int id);
    }

    public class StockMovementService : IStockMovementService
    {
        private readonly ContextoPrincipal _context;
        private readonly ILogger<StockMovementService> _logger;

        public StockMovementService(
            ContextoPrincipal context,
            ILogger<StockMovementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<StockMovementDto> CreateAsync(int empresaId, int productId, CreateStockMovementDto dto, string userId)
        {
            // Obtener el producto de la empresa
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.EmpresaId == empresaId);
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

        public async Task<IEnumerable<StockMovementDto>> GetByProductIdAsync(int empresaId, int productId)
        {
            var movements = await _context.StockMovements
                .Where(sm => sm.ProductId == productId && sm.Product!.EmpresaId == empresaId)
                .OrderByDescending(sm => sm.Timestamp)
                .ToListAsync();

            return movements.Select(MapToDto);
        }

        public async Task<StockMovementDto?> GetByIdAsync(int empresaId, int id)
        {
            var movement = await _context.StockMovements
                .FirstOrDefaultAsync(sm => sm.Id == id && sm.Product!.EmpresaId == empresaId);

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
