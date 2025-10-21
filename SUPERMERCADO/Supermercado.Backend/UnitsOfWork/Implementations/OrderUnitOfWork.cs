using Microsoft.EntityFrameworkCore;
using Supermercado.Backend.Data;
using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.UnitsOfWork.Implementations;

public class OrderUnitOfWork : IOrderUnitOfWork
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly DataContext _context;

    public OrderUnitOfWork(IOrderRepository orderRepository, IProductRepository productRepository, DataContext context)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _context = context;
    }

    public async Task<ActionResponse<OrderDTO>> GetOrderByIdAsync(int id)
    {
        var response = await _orderRepository.GetOrderWithDetailsAsync(id);
        if (!response.WasSuccess)
        {
            return new ActionResponse<OrderDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        var order = response.Result!;
        var dto = MapToDTO(order);

        return new ActionResponse<OrderDTO>
        {
            WasSuccess = true,
            Result = dto
        };
    }

    public async Task<ActionResponse<PaginationDTO<OrderDTO>>> GetOrdersAsync(int page, int pageSize, string? status = null, int? customerId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var response = await _orderRepository.GetPaginatedAsync(page, pageSize, status, customerId, startDate, endDate);
        if (!response.WasSuccess)
        {
            return new ActionResponse<PaginationDTO<OrderDTO>>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        var pagination = response.Result!;
        var dtos = pagination.Items.Select(MapToDTO).ToList();

        return new ActionResponse<PaginationDTO<OrderDTO>>
        {
            WasSuccess = true,
            Result = new PaginationDTO<OrderDTO>
            {
                Items = dtos,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                Total = pagination.Total
            }
        };
    }

    public async Task<ActionResponse<OrderDTO>> CreateOrderAsync(CreateOrderDTO dto)
    {
        try
        {
            // Validar cliente
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
            {
                return new ActionResponse<OrderDTO>
                {
                    WasSuccess = false,
                    Message = "Cliente no encontrado"
                };
            }

            if (!customer.IsActive)
            {
                return new ActionResponse<OrderDTO>
                {
                    WasSuccess = false,
                    Message = "El cliente está inactivo"
                };
            }

            // Validar vendedor si se proporciona
            if (dto.SellerId.HasValue)
            {
                var seller = await _context.Sellers.FindAsync(dto.SellerId.Value);
                if (seller == null)
                {
                    return new ActionResponse<OrderDTO>
                    {
                        WasSuccess = false,
                        Message = "Vendedor no encontrado"
                    };
                }

                if (!seller.IsActive)
                {
                    return new ActionResponse<OrderDTO>
                    {
                        WasSuccess = false,
                        Message = "El vendedor está inactivo"
                    };
                }
            }

            // Crear pedido
            var orderNumber = await _orderRepository.GenerateOrderNumberAsync();
            var order = new Order
            {
                Number = orderNumber,
                CustomerId = dto.CustomerId,
                SellerId = dto.SellerId,
                Status = "NEW",
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                OrderLines = new List<OrderLine>()
            };

            decimal subtotal = 0;
            decimal tax = 0;

            // Crear líneas de pedido
            foreach (var lineDto in dto.OrderLines)
            {
                var product = await _context.Products.FindAsync(lineDto.ProductId);
                if (product == null)
                {
                    return new ActionResponse<OrderDTO>
                    {
                        WasSuccess = false,
                        Message = $"Producto con ID {lineDto.ProductId} no encontrado"
                    };
                }

                if (!product.IsActive)
                {
                    return new ActionResponse<OrderDTO>
                    {
                        WasSuccess = false,
                        Message = $"El producto {product.Name} está inactivo"
                    };
                }

                var lineSubtotal = product.Price * lineDto.Qty;
                var lineTax = lineSubtotal * (product.TaxRatePct / 100);
                var lineTotal = lineSubtotal + lineTax;

                var orderLine = new OrderLine
                {
                    ProductId = lineDto.ProductId,
                    Qty = lineDto.Qty,
                    UnitPrice = product.Price,
                    TaxRatePct = product.TaxRatePct,
                    LineTotal = lineTotal,
                    LineTax = lineTax
                };

                order.OrderLines.Add(orderLine);
                subtotal += lineSubtotal;
                tax += lineTax;
            }

            order.Subtotal = subtotal;
            order.Tax = tax;
            order.Total = subtotal + tax;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Recargar con relaciones
            var result = await _orderRepository.GetOrderWithDetailsAsync(order.Id);
            return new ActionResponse<OrderDTO>
            {
                WasSuccess = true,
                Result = MapToDTO(result.Result!)
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<OrderDTO>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<OrderDTO>> UpdateOrderAsync(int id, UpdateOrderDTO dto)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.OrderLines)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return new ActionResponse<OrderDTO>
                {
                    WasSuccess = false,
                    Message = "Pedido no encontrado"
                };
            }

            if (order.Status != "NEW")
            {
                return new ActionResponse<OrderDTO>
                {
                    WasSuccess = false,
                    Message = $"Solo se pueden editar pedidos en estado NEW. Estado actual: {order.Status}"
                };
            }

            // Validar cliente
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
            {
                return new ActionResponse<OrderDTO>
                {
                    WasSuccess = false,
                    Message = "Cliente no encontrado"
                };
            }

            // Eliminar líneas existentes
            _context.OrderLines.RemoveRange(order.OrderLines!);

            // Actualizar datos del pedido
            order.CustomerId = dto.CustomerId;
            order.Notes = dto.Notes;

            decimal subtotal = 0;
            decimal tax = 0;

            // Crear nuevas líneas
            foreach (var lineDto in dto.OrderLines)
            {
                var product = await _context.Products.FindAsync(lineDto.ProductId);
                if (product == null)
                {
                    return new ActionResponse<OrderDTO>
                    {
                        WasSuccess = false,
                        Message = $"Producto con ID {lineDto.ProductId} no encontrado"
                    };
                }

                var lineSubtotal = product.Price * lineDto.Qty;
                var lineTax = lineSubtotal * (product.TaxRatePct / 100);
                var lineTotal = lineSubtotal + lineTax;

                var orderLine = new OrderLine
                {
                    OrderId = order.Id,
                    ProductId = lineDto.ProductId,
                    Qty = lineDto.Qty,
                    UnitPrice = product.Price,
                    TaxRatePct = product.TaxRatePct,
                    LineTotal = lineTotal,
                    LineTax = lineTax
                };

                _context.OrderLines.Add(orderLine);
                subtotal += lineSubtotal;
                tax += lineTax;
            }

            order.Subtotal = subtotal;
            order.Tax = tax;
            order.Total = subtotal + tax;

            await _context.SaveChangesAsync();

            // Recargar con relaciones
            var result = await _orderRepository.GetOrderWithDetailsAsync(order.Id);
            return new ActionResponse<OrderDTO>
            {
                WasSuccess = true,
                Result = MapToDTO(result.Result!)
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<OrderDTO>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<OrderDTO>> ConfirmOrderAsync(int id)
    {
        var response = await _orderRepository.ConfirmOrderAsync(id);
        if (!response.WasSuccess)
        {
            return new ActionResponse<OrderDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<OrderDTO>
        {
            WasSuccess = true,
            Result = MapToDTO(response.Result!)
        };
    }

    public async Task<ActionResponse<OrderDTO>> CancelOrderAsync(int id, string userEmail)
    {
        var response = await _orderRepository.CancelOrderAsync(id, userEmail);
        if (!response.WasSuccess)
        {
            return new ActionResponse<OrderDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<OrderDTO>
        {
            WasSuccess = true,
            Result = MapToDTO(response.Result!)
        };
    }

    private OrderDTO MapToDTO(Order order)
    {
        return new OrderDTO
        {
            Id = order.Id,
            Number = order.Number,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.Name,
            SellerId = order.SellerId,
            SellerName = order.Seller?.FullName,
            Status = order.Status,
            Subtotal = order.Subtotal,
            Tax = order.Tax,
            Total = order.Total,
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            ConfirmedAt = order.ConfirmedAt,
            OrderLines = order.OrderLines?.Select(ol => new OrderLineDTO
            {
                Id = ol.Id,
                ProductId = ol.ProductId,
                ProductName = ol.Product?.Name,
                ProductSku = ol.Product?.Sku,
                Qty = ol.Qty,
                UnitPrice = ol.UnitPrice,
                TaxRatePct = ol.TaxRatePct,
                LineTotal = ol.LineTotal,
                LineTax = ol.LineTax
            }).ToList()
        };
    }
}
