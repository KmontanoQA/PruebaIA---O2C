using Supermercado.Shared.DTOs;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.UnitsOfWork.Interfaces;

public interface IInvoiceUnitOfWork
{
    Task<ActionResponse<InvoiceDTO>> GetInvoiceByIdAsync(int id);
    Task<ActionResponse<PaginationDTO<InvoiceDTO>>> GetInvoicesAsync(int page, int pageSize, string? status = null, int? customerId = null);
    Task<ActionResponse<InvoiceDTO>> CreateInvoiceAsync(CreateInvoiceDTO dto);
    Task<ActionResponse<InvoiceDTO>> CancelInvoiceAsync(int id);
}
