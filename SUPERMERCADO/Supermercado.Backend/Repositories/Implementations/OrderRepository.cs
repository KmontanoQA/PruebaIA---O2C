using Microsoft.EntityFrameworkCore;
using Supermercado.Backend.Data;
using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Implementations;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly DataContext _context;

    public OrderRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<Order>> GetOrderWithDetailsAsync(int id)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Seller)
                .Include(o => o.OrderLines!)
                    .ThenInclude(ol => ol.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "Pedido no encontrado"
                };
            }

            return new ActionResponse<Order>
            {
                WasSuccess = true,
                Result = order
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<Order>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<PaginationDTO<Order>>> GetPaginatedAsync(int page, int pageSize, string? status = null, int? customerId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Seller)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            if (customerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == customerId.Value);
            }

            // US-ORD-5: Filtros de fecha
            if (startDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                // Incluir todo el día final
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(o => o.CreatedAt <= endOfDay);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ActionResponse<PaginationDTO<Order>>
            {
                WasSuccess = true,
                Result = new PaginationDTO<Order>
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
            return new ActionResponse<PaginationDTO<Order>>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<Order>> ConfirmOrderAsync(int orderId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderLines!)
                    .ThenInclude(ol => ol.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "Pedido no encontrado"
                };
            }

            if (order.Status != "NEW")
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = $"El pedido no puede ser confirmado. Estado actual: {order.Status}"
                };
            }

            // Verificar stock y actualizar inventario
            foreach (var line in order.OrderLines!)
            {
                var product = line.Product!;
                if (product.StockQty < line.Qty)
                {
                    await transaction.RollbackAsync();
                    return new ActionResponse<Order>
                    {
                        WasSuccess = false,
                        Message = $"Stock insuficiente para {product.Name}. Disponible: {product.StockQty}, Requerido: {line.Qty}",
                        StatusCode = 409 // Conflict - según US-ORD-2
                    };
                }

                // Reducir stock
                product.StockQty -= line.Qty;

                // Crear movimiento de inventario
                var inventoryMove = new InventoryMove
                {
                    ProductId = product.Id,
                    RefType = "ORDER",
                    RefId = order.Id,
                    QtyDelta = -line.Qty,
                    StockAfter = product.StockQty,
                    Notes = $"Pedido confirmado: {order.Number}",
                    CreatedAt = DateTime.UtcNow
                };
                _context.InventoryMoves.Add(inventoryMove);
            }

            var previousStatus = order.Status;
            order.Status = "CONFIRMED";
            order.ConfirmedAt = DateTime.UtcNow;

            // Registrar auditoría de confirmación
            var audit = new OrderAudit
            {
                OrderId = order.Id,
                Action = "CONFIRMED",
                UserEmail = "system", // Se actualizará cuando se pase el usuario desde el controlador
                PreviousStatus = previousStatus,
                NewStatus = "CONFIRMED",
                Notes = $"Pedido {order.Number} confirmado",
                CreatedAt = DateTime.UtcNow
            };
            _context.OrderAudits.Add(audit);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ActionResponse<Order>
            {
                WasSuccess = true,
                Result = order
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new ActionResponse<Order>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<Order>> CancelOrderAsync(int orderId, string userEmail)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderLines!)
                    .ThenInclude(ol => ol.Product)
                .Include(o => o.Invoice)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "Pedido no encontrado"
                };
            }

            // US-ORD-3: Solo pedidos CONFIRMED pueden anularse
            if (order.Status != "CONFIRMED")
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = $"Solo se pueden cancelar pedidos confirmados. Estado actual: {order.Status}"
                };
            }

            if (order.Status == "INVOICED")
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "No se puede cancelar un pedido facturado. Debe cancelar la factura primero."
                };
            }

            var previousStatus = order.Status;

            // Reversar inventario
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
                    Notes = $"Pedido cancelado: {order.Number}",
                    CreatedAt = DateTime.UtcNow
                };
                _context.InventoryMoves.Add(inventoryMove);
            }

            order.Status = "CANCELLED";
            order.CancelledAt = DateTime.UtcNow;

            // US-ORD-3: Registrar auditoría con usuario, fecha y acción
            var audit = new OrderAudit
            {
                OrderId = order.Id,
                Action = "CANCELLED",
                UserEmail = userEmail,
                PreviousStatus = previousStatus,
                NewStatus = "CANCELLED",
                Notes = $"Pedido {order.Number} cancelado por {userEmail}",
                CreatedAt = DateTime.UtcNow
            };
            _context.OrderAudits.Add(audit);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ActionResponse<Order>
            {
                WasSuccess = true,
                Result = order
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new ActionResponse<Order>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<string> GenerateOrderNumberAsync()
    {
        var lastOrder = await _context.Orders
            .OrderByDescending(o => o.Id)
            .FirstOrDefaultAsync();

        var nextNumber = lastOrder == null ? 1 : lastOrder.Id + 1;
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{nextNumber:D6}";
    }
}
