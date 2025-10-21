using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supermercado.Shared.Entities;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Number { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int CustomerId { get; set; }

    public int? SellerId { get; set; } // Vendedor que creó el pedido

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Status { get; set; } = "NEW"; // NEW, CONFIRMED, INVOICED, CANCELLED

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Tax { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; } = 0;

    [MaxLength(500, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ConfirmedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    // Navigation properties
    public Customer? Customer { get; set; }
    public Seller? Seller { get; set; }
    public ICollection<OrderLine>? OrderLines { get; set; }
    public Invoice? Invoice { get; set; }
}
