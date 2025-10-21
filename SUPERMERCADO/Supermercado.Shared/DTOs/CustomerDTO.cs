using System.ComponentModel.DataAnnotations;

namespace Supermercado.Shared.DTOs;

public class CustomerDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(200, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string TaxId { get; set; } = null!;

    [MaxLength(150, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    [EmailAddress(ErrorMessage = "El campo {0} debe ser un email válido")]
    public string? Email { get; set; }

    [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Phone { get; set; }

    [MaxLength(300, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;
}

public class CreateCustomerDTO
{
    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(200, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string TaxId { get; set; } = null!;

    [MaxLength(150, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    [EmailAddress(ErrorMessage = "El campo {0} debe ser un email válido")]
    public string? Email { get; set; }

    [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Phone { get; set; }

    [MaxLength(300, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;
}
