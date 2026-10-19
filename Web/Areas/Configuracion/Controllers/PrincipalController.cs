using InventoryManagement.Data.Base;
using InventoryManagement.Data.Dtos;
using InventoryManagement.Web.Areas.Configuracion.Models;
using InventoryManagement.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Web.Areas.Configuracion.Controllers
{
	/// <summary>
	/// Panel inicial de la empresa activa
	/// </summary>
	[Area("Configuracion")]
	[Route("Configuracion/Resumen")]
	public class PrincipalController : BaseMvcController
	{
		public PrincipalController(ContextoPrincipal context, ILoggerFactory logger) : base(context, logger)
		{
		}

		[HttpGet]
		[Route("Inicio")]
		public async Task<IActionResult> Inicio()
		{
			try
			{
				var empresa = EmpresaSesion;
				var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

				var productos = _context.Products.AsNoTracking().Where(p => p.EmpresaId == empresa.Id);

				var model = new DashboardViewModel
				{
					TotalProductos = await productos.CountAsync(),
					ProductosStockBajo = await productos.CountAsync(p => p.QuantityInStock <= p.MinimumStock),
					ValorInventario = await productos.SumAsync(p => p.QuantityInStock * p.UnitPrice),
					MovimientosMes = await _context.StockMovements.CountAsync(m => m.Product!.EmpresaId == empresa.Id && m.Timestamp >= inicioMes),
					StockBajo = await productos
						.Where(p => p.QuantityInStock <= p.MinimumStock)
						.OrderBy(p => p.QuantityInStock)
						.Take(10)
						.Select(p => new ProductDto
						{
							Id = p.Id,
							Name = p.Name,
							SKU = p.SKU,
							Category = p.Category,
							QuantityInStock = p.QuantityInStock,
							MinimumStock = p.MinimumStock,
							UnitPrice = p.UnitPrice
						})
						.ToListAsync()
				};

				ViewBag.NombreEmpresa = empresa.Nombre;
				return View(model);
			}
			catch (InvalidOperationException ex) when (ex.Message == Extensions.MensajeSesionCaducada)
			{
				return RedirectToAction("Inicio", "Principal", new { area = "" });
			}
		}
	}
}
