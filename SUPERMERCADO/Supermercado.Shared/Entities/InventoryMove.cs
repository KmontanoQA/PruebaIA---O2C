using System.ComponentModel.DataAnnotations;

namespace Supermercado.Shared.Entities;

public class InventoryMove
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string RefType { get; set; } = null!; // ORDER, ADJUST, RETURN, INITIAL

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int RefId { get; set; } // ID del pedido, ajuste, etc.

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int QtyDelta { get; set; } // Positivo = entrada, Negativo = salida

    public int StockAfter { get; set; } // Stock después del movimiento

    [MaxLength(500, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Product? Product { get; set; }
}
