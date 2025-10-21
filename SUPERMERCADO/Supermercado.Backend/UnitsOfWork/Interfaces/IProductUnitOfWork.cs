using Supermercado.Shared.DTOs;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.UnitsOfWork.Interfaces;

public interface IProductUnitOfWork
{
    Task<ActionResponse<ProductDTO>> GetByIdAsync(int id);
    Task<ActionResponse<IEnumerable<ProductDTO>>> GetAllAsync();
    Task<ActionResponse<PaginationDTO<ProductDTO>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null);
    Task<ActionResponse<ProductDTO>> GetBySkuAsync(string sku);
    Task<ActionResponse<ProductDTO>> CreateAsync(CreateProductDTO dto);
    Task<ActionResponse<ProductDTO>> UpdateAsync(int id, ProductDTO dto);
    Task<ActionResponse<bool>> DeleteAsync(int id);
}
