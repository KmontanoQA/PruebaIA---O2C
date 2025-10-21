using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supermercado.Shared.Entities;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Number { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Status { get; set; } = "PENDING"; // PENDING, PAID, CANCELLED

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Tax { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; } = 0;

    public DateTime DueDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PaidAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    [MaxLength(500, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Notes { get; set; }

    // Navigation properties
    public Order? Order { get; set; }
    public Customer? Customer { get; set; }
}
