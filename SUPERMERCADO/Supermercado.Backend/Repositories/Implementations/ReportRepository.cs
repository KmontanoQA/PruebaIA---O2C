using Microsoft.EntityFrameworkCore;
using Supermercado.Backend.Data;
using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Implementations;

public class ReportRepository : IReportRepository
{
    private readonly DataContext _context;

    public ReportRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<ActionResponse<SalesReportDTO>> GetSalesReportAsync(DateTime dateFrom, DateTime dateTo)
    {
        try
        {
            var invoices = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Order)
                .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.Status != "CANCELLED")
                .OrderBy(i => i.CreatedAt)
                .ToListAsync();

            var report = new SalesReportDTO
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                TotalInvoices = invoices.Count,
                TotalSales = invoices.Sum(i => i.Total),
                TotalTax = invoices.Sum(i => i.Tax),
                TotalNet = invoices.Sum(i => i.Subtotal),
                Details = invoices.Select(i => new SalesReportLineDTO
                {
                    Date = i.CreatedAt,
                    InvoiceNumber = i.Number,
                    OrderNumber = i.Order?.Number ?? "",
                    CustomerName = i.Customer?.Name ?? "",
                    Subtotal = i.Subtotal,
                    Tax = i.Tax,
                    Total = i.Total,
                    Status = i.Status
                }).ToList()
            };

            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo && o.Status == "CONFIRMED")
                .CountAsync();

            report.TotalOrders = orders;

            return new ActionResponse<SalesReportDTO>
            {
                WasSuccess = true,
                Result = report
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<SalesReportDTO>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<ReceivablesReportDTO>> GetReceivablesReportAsync(int? customerId, DateTime? asOf)
    {
        try
        {
            var asOfDate = asOf ?? DateTime.UtcNow;
            var query = _context.Invoices
                .Include(i => i.Customer)
                .Where(i => i.Status == "PENDING");

            if (customerId.HasValue)
            {
                query = query.Where(i => i.CustomerId == customerId.Value);
            }

            var invoices = await query.ToListAsync();

            var report = new ReceivablesReportDTO
            {
                AsOf = asOfDate,
                CustomerId = customerId,
                CustomerName = customerId.HasValue 
                    ? (await _context.Customers.FindAsync(customerId.Value))?.Name 
                    : "Todos los clientes",
                TotalPending = invoices.Sum(i => i.Total),
                TotalOverdue = invoices.Where(i => i.DueDate < asOfDate).Sum(i => i.Total),
                Details = invoices.Select(i => new ReceivablesReportLineDTO
                {
                    InvoiceId = i.Id,
                    InvoiceNumber = i.Number,
                    CustomerName = i.Customer?.Name ?? "",
                    DueDate = i.DueDate,
                    Total = i.Total,
                    DaysOverdue = i.DueDate < asOfDate ? (int)(asOfDate - i.DueDate).TotalDays : 0,
                    Status = i.Status
                }).OrderBy(x => x.DueDate).ToList()
            };

            return new ActionResponse<ReceivablesReportDTO>
            {
                WasSuccess = true,
                Result = report
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<ReceivablesReportDTO>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<List<InventoryReportDTO>>> GetInventoryReportAsync()
    {
        try
        {
            var products = await _context.Products
                .Include(p => p.InventoryMoves)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            var report = products.Select(p => new InventoryReportDTO
            {
                ProductId = p.Id,
                Sku = p.Sku,
                ProductName = p.Name,
                CurrentStock = p.StockQty,
                UnitPrice = p.Price,
                TotalValue = p.StockQty * p.Price,
                RecentMoves = p.InventoryMoves?
                    .OrderByDescending(im => im.CreatedAt)
                    .Take(10)
                    .Select(im => new InventoryMoveDTO
                    {
                        Id = im.Id,
                        RefType = im.RefType,
                        RefId = im.RefId,
                        QtyDelta = im.QtyDelta,
                        StockAfter = im.StockAfter,
                        CreatedAt = im.CreatedAt,
                        Notes = im.Notes
                    }).ToList() ?? new List<InventoryMoveDTO>()
            }).ToList();

            return new ActionResponse<List<InventoryReportDTO>>
            {
                WasSuccess = true,
                Result = report
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<List<InventoryReportDTO>>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }
}
