using System.ComponentModel.DataAnnotations;

namespace Supermercado.Shared.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(150, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    [EmailAddress(ErrorMessage = "El campo {0} debe ser un email válido")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(255, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string PasswordHash { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Role { get; set; } = null!; // Admin, User, Seller

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? FullName { get; set; }
}
