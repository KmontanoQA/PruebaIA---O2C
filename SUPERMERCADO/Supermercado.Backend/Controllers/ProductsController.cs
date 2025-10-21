using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using Supermercado.Shared.DTOs;

namespace Supermercado.Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductUnitOfWork _unitOfWork;

    public ProductsController(IProductUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Obtener todos los productos
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
    /// Obtener productos paginados
    /// </summary>
    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool? isActive = null)
    {
        var response = await _unitOfWork.GetPaginatedAsync(page, pageSize, isActive);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        Response.Headers.Append("X-Total-Count", response.Result!.Total.ToString());
        return Ok(response.Result);
    }

    /// <summary>
    /// Obtener producto por ID
    /// </summary>
    [HttpGet("{id}", Name = "GetProductById")]
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
    /// Obtener producto por SKU
    /// </summary>
    [HttpGet("sku/{sku}")]
    public async Task<IActionResult> GetBySku(string sku)
    {
        var response = await _unitOfWork.GetBySkuAsync(sku);
        if (!response.WasSuccess)
        {
            return NotFound(response.Message);
        }
        return Ok(response.Result);
    }

    /// <summary>
    /// Crear nuevo producto
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateProductDTO dto)
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

        return CreatedAtRoute("GetProductById", new { id = response.Result!.Id }, response.Result);
    }

    /// <summary>
    /// Actualizar producto
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(int id, [FromBody] ProductDTO dto)
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
    /// Eliminar producto
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
