using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.UnitsOfWork.Interfaces;

public interface IOrderUnitOfWork
{
    Task<ActionResponse<OrderDTO>> GetOrderByIdAsync(int id);
    Task<ActionResponse<PaginationDTO<OrderDTO>>> GetOrdersAsync(int page, int pageSize, string? status = null, int? customerId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<ActionResponse<OrderDTO>> CreateOrderAsync(CreateOrderDTO dto);
    Task<ActionResponse<OrderDTO>> UpdateOrderAsync(int id, UpdateOrderDTO dto);
    Task<ActionResponse<OrderDTO>> ConfirmOrderAsync(int id);
    Task<ActionResponse<OrderDTO>> CancelOrderAsync(int id, string userEmail);
}
