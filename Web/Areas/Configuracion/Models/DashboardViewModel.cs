using InventoryManagement.Data.Dtos;

namespace InventoryManagement.Web.Areas.Configuracion.Models
{
	public class DashboardViewModel
	{
		public int TotalProductos { get; set; }
		public int ProductosStockBajo { get; set; }
		public int MovimientosMes { get; set; }
		public decimal ValorInventario { get; set; }
		public List<ProductDto> StockBajo { get; set; } = new();
	}
}
