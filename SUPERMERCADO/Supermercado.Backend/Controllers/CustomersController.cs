using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using Supermercado.Shared.DTOs;

namespace Supermercado.Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerUnitOfWork _unitOfWork;

    public CustomersController(ICustomerUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Obtener todos los clientes
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var response = await _unitOfWork.GetAllAsync();
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }
        return Ok(response.Result);
    }

    /// <summary>
    /// Obtener cliente por ID
    /// </summary>
    [HttpGet("{id}", Name = "GetCustomerById")]
    public async Task<IActionResult> GetAsync(int id)
    {
        var response = await _unitOfWork.GetByIdAsync(id);
        if (!response.WasSuccess)
        {
            return NotFound(response.Message);
        }
        return Ok(response.Result);
    }

    /// <summary>
    /// Crear nuevo cliente
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateCustomerDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _unitOfWork.CreateAsync(dto);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return CreatedAtRoute("GetCustomerById", new { id = response.Result!.Id }, response.Result);
    }

    /// <summary>
    /// Actualizar cliente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(int id, [FromBody] CustomerDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _unitOfWork.UpdateAsync(id, dto);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Result);
    }

    /// <summary>
    /// Eliminar cliente
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var response = await _unitOfWork.DeleteAsync(id);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }
        return NoContent();
    }
}

