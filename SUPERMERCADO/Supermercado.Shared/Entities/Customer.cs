using System.ComponentModel.DataAnnotations;

namespace Supermercado.Shared.Entities;

public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(200, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string TaxId { get; set; } = null!; // NIT, RUC, RFC, etc.

    [MaxLength(150, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    [EmailAddress(ErrorMessage = "El campo {0} debe ser un email válido")]
    public string? Email { get; set; }

    [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Phone { get; set; }

    [MaxLength(300, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Order>? Orders { get; set; }
    public ICollection<Invoice>? Invoices { get; set; }
}
