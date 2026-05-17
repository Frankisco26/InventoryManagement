using InventoryManagement.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Data.Dtos
{
    public class StockMovementDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class CreateStockMovementDto
    {
        [Required(ErrorMessage = "El tipo de movimiento es obligatorio")]
        public StockMovementType Type { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "La razón del movimiento es obligatoria")]
        [StringLength(500, MinimumLength = 2, 
            ErrorMessage = "La razón debe tener entre 2 y 500 caracteres")]
        public string Reason { get; set; } = string.Empty;
    }
}
