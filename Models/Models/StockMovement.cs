using System;

namespace InventoryManagement.Models
{
    public enum StockMovementType
    {
        Inbound = 0,   // Entrada de stock
        Outbound = 1   // Salida de stock
    }

    public class StockMovement
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public StockMovementType Type { get; set; }

        public int Quantity { get; set; }

        public string Reason { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        // Para auditoría
        public string? CreatedBy { get; set; }

        // Relación con el producto
        public Product? Product { get; set; }
    }
}
