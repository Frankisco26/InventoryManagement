using InventoryManagement.Data.Base;
using InventoryManagement.Data.Dtos;
using InventoryManagement.Data.Services;
using InventoryManagement.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Modulos = InventoryManagement.Models.Utilitarios.Areas;

namespace InventoryManagement.Web.Areas.Stock.Controllers
{
	[Area("Stock")]
	[Route("Stock/Movimientos")]
	[Authorize(nameof(Modulos.Stock))]
	public class MovimientosController : BaseMvcController
	{
		private readonly IProductService _productService;
		private readonly IStockMovementService _stockMovementService;

		public MovimientosController(ContextoPrincipal context, IProductService productService, IStockMovementService stockMovementService, ILoggerFactory logger) : base(context, logger)
		{
			_productService = productService;
			_stockMovementService = stockMovementService;
		}

		[Route("Inicio")]
		public async Task<IActionResult> Inicio(int? productoId)
		{
			var productos = await _productService.GetAllAsync(EmpresaSesion.Id);
			ViewBag.Productos = new SelectList(productos.Select(p => new { p.Id, Nombre = $"{p.SKU} - {p.Name}" }), "Id", "Nombre", productoId);
			ViewBag.ProductoId = productoId;
			return View(EmpresaSesion);
		}

		[HttpPost]
		[Route("Producto")]
		public async Task<IActionResult> Producto(int id)
		{
			var empresaId = EmpresaSesion.Id;
			var producto = await _productService.GetByIdAsync(empresaId, id);
			if (producto == null)
				return Json(new { tipo = "e", mensaje = "Producto no encontrado." });

			var movimientos = await _stockMovementService.GetByProductIdAsync(empresaId, id);
			return Json(new { tipo = "s", producto, movimientos });
		}

		[HttpPost]
		[Route("Registrar")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Registrar(int productoId, CreateStockMovementDto modelo)
		{
			var empresa = EmpresaSesion;
			if (!empresa.PermiteCrear)
				return Json(new { tipo = "e", mensaje = "Su rol no tiene permiso para crear." });

			if (!ModelState.IsValid)
			{
				var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).Distinct();
				return Json(new { tipo = "e", mensaje = string.Join(" ", errores) });
			}

			try
			{
				await _stockMovementService.CreateAsync(empresa.Id, productoId, modelo, UsuarioId.ToString());
				return Json(new { tipo = "s", mensaje = "Movimiento registrado exitosamente" });
			}
			catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
			{
				return Json(new { tipo = "e", mensaje = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return Json(new { tipo = "e", mensaje = "No se pudo registrar el movimiento." });
			}
		}
	}
}
