using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supermercado.Shared.Entities;

public class Product
{
    [Key]
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
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRatePct { get; set; } = 0; // Porcentaje de impuesto (ej: 19.00 para IVA 19%)

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int StockQty { get; set; } = 0;

    public int? CategoriaId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Categoria_Producto? Categoria { get; set; }
    public ICollection<OrderLine>? OrderLines { get; set; }
    public ICollection<InventoryMove>? InventoryMoves { get; set; }
}
