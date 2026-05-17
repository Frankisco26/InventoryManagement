using InventoryManagement.Data.Dtos;
using InventoryManagement.Data.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagement.Web.Controllers
{
    [ApiController]
    [Route("api/products/{productId}/movements")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StockMovementsController : ControllerBase
    {
        private readonly IStockMovementService _stockMovementService;
        private readonly IProductService _productService;
        private readonly ILogger<StockMovementsController> _logger;

        public StockMovementsController(
            IStockMovementService stockMovementService,
            IProductService productService,
            ILogger<StockMovementsController> logger)
        {
            _stockMovementService = stockMovementService;
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener historial de movimientos de un producto
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StockMovementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetMovements([FromRoute] int productId)
        {
            try
            {
                // Validar que el producto existe
                if (!await _productService.ProductExistsAsync(productId))
                    return NotFound(new { message = $"Producto con ID {productId} no encontrado" });

                var movements = await _stockMovementService.GetByProductIdAsync(productId);
                return Ok(movements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener movimientos del producto {productId}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error al obtener movimientos" });
            }
        }

        /// <summary>
        /// Registrar un nuevo movimiento de stock
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(StockMovementDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StockMovementDto>> CreateMovement(
            [FromRoute] int productId,
            [FromBody] CreateStockMovementDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Validar que el producto existe
                if (!await _productService.ProductExistsAsync(productId))
                    return NotFound(new { message = $"Producto con ID {productId} no encontrado" });

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "sistema";
                var movement = await _stockMovementService.CreateAsync(productId, dto, userId);

                return CreatedAtAction(nameof(GetMovements), 
                    new { productId = movement.ProductId }, 
                    movement);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear movimiento para producto {productId}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error al registrar movimiento" });
            }
        }
    }
}
