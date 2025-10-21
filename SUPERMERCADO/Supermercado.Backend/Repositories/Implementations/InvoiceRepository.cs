using Microsoft.EntityFrameworkCore;
using Supermercado.Backend.Data;
using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Implementations;

public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
{
    private readonly DataContext _context;

    public InvoiceRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<Invoice>> GetInvoiceWithDetailsAsync(int id)
    {
        try
        {
            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Order!)
                    .ThenInclude(o => o.OrderLines!)
                    .ThenInclude(ol => ol.Product)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return new ActionResponse<Invoice>
                {
                    WasSuccess = false,
                    Message = "Factura no encontrada"
                };
            }

            return new ActionResponse<Invoice>
            {
                WasSuccess = true,
                Result = invoice
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<Invoice>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<PaginationDTO<Invoice>>> GetPaginatedAsync(int page, int pageSize, string? status = null, int? customerId = null)
    {
        try
        {
            var query = _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Order)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(i => i.Status == status);
            }

            if (customerId.HasValue)
            {
                query = query.Where(i => i.CustomerId == customerId.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ActionResponse<PaginationDTO<Invoice>>
            {
                WasSuccess = true,
                Result = new PaginationDTO<Invoice>
                {
                    Items = items,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                }
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<PaginationDTO<Invoice>>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<Invoice>> CreateFromOrderAsync(int orderId, DateTime dueDate, string? notes)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = await _context.Orders
                .Include(o => o.Invoice)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return new ActionResponse<Invoice>
                {
                    WasSuccess = false,
                    Message = "Pedido no encontrado"
                };
            }

            if (order.Status != "CONFIRMED")
            {
                return new ActionResponse<Invoice>
                {
                    WasSuccess = false,
                    Message = $"Solo se pueden facturar pedidos confirmados. Estado actual: {order.Status}"
                };
            }

            if (order.Invoice != null)
            {
                return new ActionResponse<Invoice>
                {
                    WasSuccess = false,
                    Message = "El pedido ya tiene una factura asociada"
                };
            }

            var invoiceNumber = await GenerateInvoiceNumberAsync();

            var invoice = new Invoice
            {
                Number = invoiceNumber,
                OrderId = orderId,
                CustomerId = order.CustomerId,
                Status = "PENDING",
                Subtotal = order.Subtotal,
                Tax = order.Tax,
                Total = order.Total,
                DueDate = dueDate,
                Notes = notes,
                CreatedAt = DateTime.UtcNow,
                PaidAt = DateTime.UtcNow
            };

            _context.Invoices.Add(invoice);
            order.Status = "INVOICED";

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Recargar con relaciones
            var result = await GetInvoiceWithDetailsAsync(invoice.Id);
            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new ActionResponse<Invoice>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<Invoice>> CancelInvoiceAsync(int invoiceId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var invoice = await _context.Invoices
                .Include(i => i.Order!)
                    .ThenInclude(o => o.OrderLines!)
                    .ThenInclude(ol => ol.Product)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (invoice == null)
            {
                return new ActionResponse<Invoice>
                {
                    WasSuccess = false,
                    Message = "Factura no encontrada"
                };
            }

            if (invoice.Status == "CANCELLED")
            {
                return new ActionResponse<Invoice>
                {
                    WasSuccess = false,
                    Message = "La factura ya está cancelada"
                };
            }

            if (invoice.Status == "PAID")
            {
                return new ActionResponse<Invoice>
                {
                    WasSuccess = false,
                    Message = "No se puede cancelar una factura pagada"
                };
            }

            // Reversar inventario del pedido
            var order = invoice.Order!;
            foreach (var line in order.OrderLines!)
            {
                var product = line.Product!;
                product.StockQty += line.Qty;

                var inventoryMove = new InventoryMove
                {
                    ProductId = product.Id,
                    RefType = "ORDER",
                    RefId = order.Id,
                    QtyDelta = line.Qty,
                    StockAfter = product.StockQty,
                    Notes = $"Factura cancelada: {invoice.Number}",
                    CreatedAt = DateTime.UtcNow
                };
                _context.InventoryMoves.Add(inventoryMove);
            }

            invoice.Status = "CANCELLED";
            invoice.CancelledAt = DateTime.UtcNow;
            order.Status = "CANCELLED";
            order.CancelledAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ActionResponse<Invoice>
            {
                WasSuccess = true,
                Result = invoice
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new ActionResponse<Invoice>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var lastInvoice = await _context.Invoices
            .OrderByDescending(i => i.Id)
            .FirstOrDefaultAsync();

        var nextNumber = lastInvoice == null ? 1 : lastInvoice.Id + 1;
        return $"INV-{DateTime.UtcNow:yyyyMMdd}-{nextNumber:D6}";
    }
}
