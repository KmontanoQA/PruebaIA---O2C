using Microsoft.AspNetCore.Mvc;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using Supermercado.Shared.DTOs;

namespace Supermercado.Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthUnitOfWork _authUnitOfWork;

    public AuthController(IAuthUnitOfWork authUnitOfWork)
    {
        _authUnitOfWork = authUnitOfWork;
    }

    /// <summary>
    /// Autenticar usuario y obtener token JWT
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authUnitOfWork.LoginAsync(dto);
        if (!response.WasSuccess)
        {
            return Unauthorized(new { message = response.Message });
        }

        return Ok(response.Result);
    }

    /// <summary>
    /// Cerrar sesión (cliente debe eliminar el token)
    /// </summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // En JWT stateless, el logout se maneja en el cliente eliminando el token
        return NoContent();
    }
}
