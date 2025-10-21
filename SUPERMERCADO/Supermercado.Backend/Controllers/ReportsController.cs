using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supermercado.Backend.Repositories.Interfaces;

namespace Supermercado.Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportRepository _reportRepository;

    public ReportsController(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    /// <summary>
    /// Reporte de ventas por rango de fechas
    /// </summary>
    [HttpGet("sales")]
    public async Task<IActionResult> GetSalesReport([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        var from = dateFrom ?? DateTime.UtcNow.Date.AddDays(-30);
        var to = dateTo ?? DateTime.UtcNow.Date.AddDays(1).AddSeconds(-1);

        var response = await _reportRepository.GetSalesReportAsync(from, to);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Result);
    }

    /// <summary>
    /// Reporte de cartera (facturas pendientes)
    /// </summary>
    [HttpGet("receivables")]
    public async Task<IActionResult> GetReceivablesReport([FromQuery] int? customerId, [FromQuery] DateTime? asOf)
    {
        var response = await _reportRepository.GetReceivablesReportAsync(customerId, asOf);
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Result);
    }

    /// <summary>
    /// Reporte de inventario con movimientos recientes
    /// </summary>
    [HttpGet("inventory")]
    public async Task<IActionResult> GetInventoryReport()
    {
        var response = await _reportRepository.GetInventoryReportAsync();
        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Result);
    }
}
