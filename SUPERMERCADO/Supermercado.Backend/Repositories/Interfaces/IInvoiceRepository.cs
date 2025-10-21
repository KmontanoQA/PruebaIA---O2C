using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Interfaces;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Task<ActionResponse<Invoice>> GetInvoiceWithDetailsAsync(int id);
    Task<ActionResponse<PaginationDTO<Invoice>>> GetPaginatedAsync(int page, int pageSize, string? status = null, int? customerId = null);
    Task<ActionResponse<Invoice>> CreateFromOrderAsync(int orderId, DateTime dueDate, string? notes);
    Task<ActionResponse<Invoice>> CancelInvoiceAsync(int invoiceId);
    Task<string> GenerateInvoiceNumberAsync();
}
