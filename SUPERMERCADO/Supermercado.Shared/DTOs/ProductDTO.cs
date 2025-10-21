using System.ComponentModel.DataAnnotations;

namespace Supermercado.Shared.DTOs;

public class ProductDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Sku { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(200, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Name { get; set; } = null!;

    [MaxLength(500, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public decimal Price { get; set; }

    public decimal TaxRatePct { get; set; } = 0;

    public int StockQty { get; set; } = 0;

    public int? CategoriaId { get; set; }

    public string? CategoriaNombre { get; set; }

    public bool IsActive { get; set; } = true;
}

public class CreateProductDTO
{
    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Sku { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(200, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Name { get; set; } = null!;

    [MaxLength(500, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
    public decimal Price { get; set; }

    [Range(0, 100, ErrorMessage = "El porcentaje de impuesto debe estar entre 0 y 100")]
    public decimal TaxRatePct { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
    public int StockQty { get; set; } = 0;

    public int? CategoriaId { get; set; }

    public bool IsActive { get; set; } = true;
}
