using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using Supermercado.Shared.DTOs;

namespace Supermercado.Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderUnitOfWork _orderUnitOfWork;

    public OrdersController(IOrderUnitOfWork orderUnitOfWork)
    {
        _orderUnitOfWork = orderUnitOfWork;
    }

    /// <summary>
    /// Obtener pedidos paginados con filtros opcionales
    /// US-ORD-5: Soporta filtros por estado, cliente y rango de fechas
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] string? status = null, 
        [FromQuery] int? customerId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var response = await _orderUnitOfWork.GetOrdersAsync(page, pageSize, status, customerId, startDate, endDate);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        Response.Headers.Append("X-Total-Count", response.Result!.Total.ToString());
        return Ok(response.Result);
    }

    /// <summary>
    /// Obtener pedido por ID con detalles completos
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var response = await _orderUnitOfWork.GetOrderByIdAsync(id);
        if (!response.WasSuccess)
        {
            return NotFound(response.Message);
        }
        return Ok(response.Result);
    }

    /// <summary>
    /// Crear nuevo pedido
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _orderUnitOfWork.CreateOrderAsync(dto);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return CreatedAtAction(nameof(GetOrder), new { id = response.Result!.Id }, response.Result);
    }

    /// <summary>
    /// Actualizar pedido existente (solo en estado NEW)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _orderUnitOfWork.UpdateOrderAsync(id, dto);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Result);
    }

    /// <summary>
    /// Confirmar pedido (verifica stock y crea movimientos de inventario)
    /// </summary>
    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> ConfirmOrder(int id)
    {
        var response = await _orderUnitOfWork.ConfirmOrderAsync(id);
        if (!response.WasSuccess)
        {
            // Retornar 409 Conflict si hay problemas de stock según US-ORD-2
            if (response.StatusCode == 409)
            {
                return Conflict(new { message = response.Message });
            }
            return BadRequest(response.Message);
        }

        return Ok(response.Result);
    }

    /// <summary>
    /// Cancelar pedido (reversa inventario si estaba confirmado)
    /// US-ORD-3: Solo pedidos CONFIRMED, registra auditoría con usuario
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        // Obtener email del usuario autenticado desde el token JWT
        var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value ?? "unknown";
        
        var response = await _orderUnitOfWork.CancelOrderAsync(id, userEmail);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Result);
    }
}
