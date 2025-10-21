using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using Supermercado.Shared.DTOs;

namespace Supermercado.Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceUnitOfWork _invoiceUnitOfWork;

    public InvoicesController(IInvoiceUnitOfWork invoiceUnitOfWork)
    {
        _invoiceUnitOfWork = invoiceUnitOfWork;
    }

    /// <summary>
    /// Obtener facturas paginadas con filtros opcionales
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetInvoices(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] string? status = null, 
        [FromQuery] int? customerId = null)
    {
        var response = await _invoiceUnitOfWork.GetInvoicesAsync(page, pageSize, status, customerId);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        Response.Headers.Append("X-Total-Count", response.Result!.Total.ToString());
        return Ok(response.Result);
    }

    /// <summary>
    /// Obtener factura por ID con detalles completos
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInvoice(int id)
    {
        var response = await _invoiceUnitOfWork.GetInvoiceByIdAsync(id);
        if (!response.WasSuccess)
        {
            return NotFound(response.Message);
        }
        return Ok(response.Result);
    }

    /// <summary>
    /// Crear factura desde un pedido confirmado
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _invoiceUnitOfWork.CreateInvoiceAsync(dto);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return CreatedAtAction(nameof(GetInvoice), new { id = response.Result!.Id }, response.Result);
    }

    /// <summary>
    /// Cancelar factura (reversa inventario y cancela el pedido)
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelInvoice(int id)
    {
        var response = await _invoiceUnitOfWork.CancelInvoiceAsync(id);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Result);
    }
}
