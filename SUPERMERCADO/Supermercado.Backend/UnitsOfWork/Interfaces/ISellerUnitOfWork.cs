using Supermercado.Shared.DTOs;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.UnitsOfWork.Interfaces;

public interface ISellerUnitOfWork
{
    Task<ActionResponse<SellerDTO>> GetByIdAsync(int id);
    Task<ActionResponse<IEnumerable<SellerDTO>>> GetAllAsync();
    Task<ActionResponse<PaginationDTO<SellerDTO>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null);
    Task<ActionResponse<SellerDTO>> CreateAsync(CreateSellerDTO dto);
    Task<ActionResponse<SellerDTO>> UpdateAsync(int id, SellerDTO dto);
    Task<ActionResponse<bool>> DeleteAsync(int id);
}
