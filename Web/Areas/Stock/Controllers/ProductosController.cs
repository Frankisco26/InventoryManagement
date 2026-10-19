using InventoryManagement.Data.Base;
using InventoryManagement.Data.Dtos;
using InventoryManagement.Data.Services;
using InventoryManagement.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modulos = InventoryManagement.Models.Utilitarios.Areas;

namespace InventoryManagement.Web.Areas.Stock.Controllers
{
	[Area("Stock")]
	[Route("Stock/Productos")]
	[Authorize(nameof(Modulos.Stock))]
	public class ProductosController : BaseMvcController
	{
		private readonly IProductService _productService;

		public ProductosController(ContextoPrincipal context, IProductService productService, ILoggerFactory logger) : base(context, logger)
		{
			_productService = productService;
		}

		[Route("Inicio")]
		public IActionResult Inicio()
		{
			return View(EmpresaSesion);
		}

		[HttpPost]
		[Route("Listado")]
		public async Task<IActionResult> Listado()
		{
			try
			{
				return Json(await _productService.GetAllAsync(EmpresaSesion.Id));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return Json(new List<ProductDto>());
			}
		}

		[HttpPost]
		[Route("Obtener")]
		public async Task<IActionResult> Obtener(int id)
		{
			var producto = await _productService.GetByIdAsync(EmpresaSesion.Id, id);
			if (producto == null)
				return Json(new { tipo = "e", mensaje = "Producto no encontrado." });

			return Json(new { tipo = "s", producto });
		}

		[HttpPost]
		[Route("CrearActualizar")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CrearActualizar(int id, CreateProductDto modelo)
		{
			var empresa = EmpresaSesion;
			try
			{
				if (id == 0 && !empresa.PermiteCrear)
					return Json(new { tipo = "e", mensaje = "Su rol no tiene permiso para crear." });
				if (id != 0 && !empresa.PermiteEditar)
					return Json(new { tipo = "e", mensaje = "Su rol no tiene permiso para editar." });

				// En edición el SKU y la cantidad no se modifican desde el formulario
				if (id != 0)
				{
					ModelState.Remove(nameof(CreateProductDto.SKU));
					ModelState.Remove(nameof(CreateProductDto.QuantityInStock));
				}

				if (!ModelState.IsValid)
				{
					var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).Distinct();
					return Json(new { tipo = "e", mensaje = string.Join(" ", errores) });
				}

				var usuario = UsuarioId.ToString();
				if (id == 0)
				{
					await _productService.CreateAsync(empresa.Id, modelo, usuario);
					return Json(new { tipo = "s", mensaje = "Producto creado correctamente." });
				}

				await _productService.UpdateAsync(empresa.Id, id, new UpdateProductDto
				{
					Name = modelo.Name,
					Category = modelo.Category,
					UnitPrice = modelo.UnitPrice,
					MinimumStock = modelo.MinimumStock
				}, usuario);
				return Json(new { tipo = "s", mensaje = "Producto actualizado correctamente." });
			}
			catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
			{
				return Json(new { tipo = "e", mensaje = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return Json(new { tipo = "e", mensaje = "No se pudo guardar el producto." });
			}
		}

		[HttpPost]
		[Route("Eliminar")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Eliminar(int id)
		{
			var empresa = EmpresaSesion;
			if (!empresa.PermiteAnular)
				return Json(new { tipo = "e", mensaje = "Su rol no tiene permiso para eliminar." });

			try
			{
				var eliminado = await _productService.DeleteAsync(empresa.Id, id);
				return eliminado
					? Json(new { tipo = "s", mensaje = "Producto eliminado." })
					: Json(new { tipo = "e", mensaje = "Producto no encontrado." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return Json(new { tipo = "e", mensaje = "No se pudo eliminar el producto." });
			}
		}
	}
}
