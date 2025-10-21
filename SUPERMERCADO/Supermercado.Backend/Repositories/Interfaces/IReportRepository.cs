using Supermercado.Shared.DTOs;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Interfaces;

public interface IReportRepository
{
    Task<ActionResponse<SalesReportDTO>> GetSalesReportAsync(DateTime dateFrom, DateTime dateTo);
    Task<ActionResponse<ReceivablesReportDTO>> GetReceivablesReportAsync(int? customerId, DateTime? asOf);
    Task<ActionResponse<List<InventoryReportDTO>>> GetInventoryReportAsync();
}
