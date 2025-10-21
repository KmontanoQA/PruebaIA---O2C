using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<ActionResponse<Order>> GetOrderWithDetailsAsync(int id);
    Task<ActionResponse<PaginationDTO<Order>>> GetPaginatedAsync(int page, int pageSize, string? status = null, int? customerId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<ActionResponse<Order>> ConfirmOrderAsync(int orderId);
    Task<ActionResponse<Order>> CancelOrderAsync(int orderId, string userEmail);
    Task<string> GenerateOrderNumberAsync();
}
