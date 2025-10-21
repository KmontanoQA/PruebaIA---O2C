using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.UnitsOfWork.Implementations;

public class InvoiceUnitOfWork : IInvoiceUnitOfWork
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceUnitOfWork(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<ActionResponse<InvoiceDTO>> GetInvoiceByIdAsync(int id)
    {
        var response = await _invoiceRepository.GetInvoiceWithDetailsAsync(id);
        if (!response.WasSuccess)
        {
            return new ActionResponse<InvoiceDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        var invoice = response.Result!;
        var dto = MapToDTO(invoice);

        return new ActionResponse<InvoiceDTO>
        {
            WasSuccess = true,
            Result = dto
        };
    }

    public async Task<ActionResponse<PaginationDTO<InvoiceDTO>>> GetInvoicesAsync(int page, int pageSize, string? status = null, int? customerId = null)
    {
        var response = await _invoiceRepository.GetPaginatedAsync(page, pageSize, status, customerId);
        if (!response.WasSuccess)
        {
            return new ActionResponse<PaginationDTO<InvoiceDTO>>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        var pagination = response.Result!;
        var dtos = pagination.Items.Select(MapToDTO).ToList();

        return new ActionResponse<PaginationDTO<InvoiceDTO>>
        {
            WasSuccess = true,
            Result = new PaginationDTO<InvoiceDTO>
            {
                Items = dtos,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                Total = pagination.Total
            }
        };
    }

    public async Task<ActionResponse<InvoiceDTO>> CreateInvoiceAsync(CreateInvoiceDTO dto)
    {
        var response = await _invoiceRepository.CreateFromOrderAsync(dto.OrderId, dto.DueDate, dto.Notes);
        if (!response.WasSuccess)
        {
            return new ActionResponse<InvoiceDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<InvoiceDTO>
        {
            WasSuccess = true,
            Result = MapToDTO(response.Result!)
        };
    }

    public async Task<ActionResponse<InvoiceDTO>> CancelInvoiceAsync(int id)
    {
        var response = await _invoiceRepository.CancelInvoiceAsync(id);
        if (!response.WasSuccess)
        {
            return new ActionResponse<InvoiceDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<InvoiceDTO>
        {
            WasSuccess = true,
            Result = MapToDTO(response.Result!)
        };
    }

    private InvoiceDTO MapToDTO(Invoice invoice)
    {
        return new InvoiceDTO
        {
            Id = invoice.Id,
            Number = invoice.Number,
            OrderId = invoice.OrderId,
            OrderNumber = invoice.Order?.Number,
            CustomerId = invoice.CustomerId,
            CustomerName = invoice.Customer?.Name,
            Status = invoice.Status,
            Subtotal = invoice.Subtotal,
            Tax = invoice.Tax,
            Total = invoice.Total,
            DueDate = invoice.DueDate,
            CreatedAt = invoice.CreatedAt,
            PaidAt = invoice.PaidAt,
            Notes = invoice.Notes
        };
    }


}
