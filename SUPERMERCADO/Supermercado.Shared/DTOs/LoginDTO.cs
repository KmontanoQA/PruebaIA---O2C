using System.ComponentModel.DataAnnotations;

namespace Supermercado.Shared.DTOs;

public class LoginDTO
{
    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    [EmailAddress(ErrorMessage = "El campo {0} debe ser un email válido")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "El campo {0} es obligatorio")]
    public string Password { get; set; } = null!;
}

public class TokenDTO
{
    public string AccessToken { get; set; } = null!;
    public int ExpiresIn { get; set; } // Segundos
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}
