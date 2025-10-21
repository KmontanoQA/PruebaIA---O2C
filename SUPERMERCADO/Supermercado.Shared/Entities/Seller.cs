using System.ComponentModel.DataAnnotations;

namespace Supermercado.Shared.Entities;

/// <summary>
/// Entidad Vendedor - Representa a los vendedores que gestionan pedidos y facturas
/// </summary>
public class Seller
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Code { get; set; } = null!; // Código único del vendedor (ej: VEND-001)

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(150, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Email { get; set; } = null!;

    [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Order>? Orders { get; set; }
}
