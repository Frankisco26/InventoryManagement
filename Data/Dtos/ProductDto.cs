using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Data.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }
        public int MinimumStock { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class CreateProductDto
    {
        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(200, MinimumLength = 2,
            ErrorMessage = "El nombre debe tener entre 2 y 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El SKU es obligatorio")]
        [StringLength(50, MinimumLength = 1,
            ErrorMessage = "El SKU debe tener entre 1 y 50 caracteres")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "La categoría debe tener entre 2 y 100 caracteres")]
        public string Category { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad en stock no puede ser negativa")]
        public int QuantityInStock { get; set; } = 0;

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal UnitPrice { get; set; }
        public int MinimumStock { get; set; }
    }

    public class UpdateProductDto
    {
        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(200, MinimumLength = 2,
            ErrorMessage = "El nombre debe tener entre 2 y 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "La categoría debe tener entre 2 y 100 caracteres")]
        public string Category { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad en stock no puede ser negativa")]
        public int QuantityInStock { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal UnitPrice { get; set; }
        public int MinimumStock { get; set; }
    }
}
