using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using Supermercado.Shared.DTOs;

namespace Supermercado.Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SellersController : ControllerBase
{
    private readonly ISellerUnitOfWork _sellerUnitOfWork;

    public SellersController(ISellerUnitOfWork sellerUnitOfWork)
    {
        _sellerUnitOfWork = sellerUnitOfWork;
    }

    /// <summary>
    /// Obtener todos los vendedores activos
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var response = await _sellerUnitOfWork.GetAllAsync();
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Result);
    }

    /// <summary>
    /// Obtener vendedores paginados con filtro opcional por estado
    /// </summary>
    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginatedAsync(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] bool? isActive = null)
    {
        var response = await _sellerUnitOfWork.GetPaginatedAsync(page, pageSize, isActive);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        Response.Headers.Append("X-Total-Count", response.Result!.Total.ToString());
        return Ok(response.Result);
    }

    /// <summary>
    /// Obtener vendedor por ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAsync(int id)
    {
        var response = await _sellerUnitOfWork.GetByIdAsync(id);
        if (!response.WasSuccess)
        {
            return NotFound(response.Message);
        }

        return Ok(response.Result);
    }

    /// <summary>
    /// Crear nuevo vendedor
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateSellerDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _sellerUnitOfWork.CreateAsync(dto);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return CreatedAtAction(nameof(GetAsync), new { id = response.Result!.Id }, response.Result);
    }

    /// <summary>
    /// Actualizar vendedor existente
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsync(int id, [FromBody] SellerDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _sellerUnitOfWork.UpdateAsync(id, dto);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Result);
    }

    /// <summary>
    /// Eliminar vendedor (soft delete recomendado)
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var response = await _sellerUnitOfWork.DeleteAsync(id);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return NoContent();
    }
}
