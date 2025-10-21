using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Interfaces;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<ActionResponse<Customer>> GetByTaxIdAsync(string taxId);
    Task<ActionResponse<PaginationDTO<Customer>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null);
}
