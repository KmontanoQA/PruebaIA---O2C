using System.ComponentModel.DataAnnotations;

namespace Supermercado.Shared.DTOs;

public class SellerDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(150, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    [EmailAddress(ErrorMessage = "El campo {0} debe ser un email válido")]
    public string Email { get; set; } = null!;

    [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Phone { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class CreateSellerDTO
{
    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(150, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    [EmailAddress(ErrorMessage = "El campo {0} debe ser un email válido")]
    public string Email { get; set; } = null!;

    [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Phone { get; set; }
}
