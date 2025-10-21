using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Interfaces;

public interface ISellerRepository : IGenericRepository<Seller>
{
    Task<ActionResponse<Seller>> GetByCodeAsync(string code);
    Task<ActionResponse<PaginationDTO<Seller>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null);
}
