using Supermercado.Shared.DTOs;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.UnitsOfWork.Interfaces;

public interface ICustomerUnitOfWork
{
    Task<ActionResponse<CustomerDTO>> GetByIdAsync(int id);
    Task<ActionResponse<IEnumerable<CustomerDTO>>> GetAllAsync();
    Task<ActionResponse<PaginationDTO<CustomerDTO>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null);
    Task<ActionResponse<CustomerDTO>> CreateAsync(CreateCustomerDTO dto);
    Task<ActionResponse<CustomerDTO>> UpdateAsync(int id, CustomerDTO dto);
    Task<ActionResponse<bool>> DeleteAsync(int id);
}
