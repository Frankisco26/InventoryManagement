
namespace InventoryManagement.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// SKU (Stock Keeping Unit) - Único para cada producto
        /// </summary>
        public string SKU { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public int QuantityInStock { get; set; }

        public int MinimumStock { get; set; }

        public decimal UnitPrice { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Relación con movimientos de stock 
        /// </summary>
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}
