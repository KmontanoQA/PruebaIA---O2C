using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<ActionResponse<Product>> GetBySkuAsync(string sku);
    Task<ActionResponse<PaginationDTO<Product>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null);
    Task<ActionResponse<bool>> UpdateStockAsync(int productId, int qtyDelta);
}
