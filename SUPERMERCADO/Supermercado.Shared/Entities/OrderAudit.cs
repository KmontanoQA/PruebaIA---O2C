using System.ComponentModel.DataAnnotations;

namespace Supermercado.Shared.Entities;

/// <summary>
/// Registro de auditoría para acciones sobre pedidos (confirmación, cancelación, etc.)
/// Cumple con US-ORD-3: registrar usuario, fecha y acción
/// </summary>
public class OrderAudit
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Action { get; set; } = null!; // CONFIRMED, CANCELLED, INVOICED, etc.

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(200, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string UserEmail { get; set; } = null!; // Email del usuario que ejecutó la acción

    public int? SellerId { get; set; } // Vendedor que ejecutó la acción (si aplica)

    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? PreviousStatus { get; set; }

    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? NewStatus { get; set; }

    [MaxLength(500, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Order? Order { get; set; }
    public Seller? Seller { get; set; }
}
