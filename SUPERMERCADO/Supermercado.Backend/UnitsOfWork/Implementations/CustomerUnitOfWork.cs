using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.UnitsOfWork.Implementations;

public class CustomerUnitOfWork : ICustomerUnitOfWork
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IGenericRepository<Customer> _genericRepository;

    public CustomerUnitOfWork(ICustomerRepository customerRepository, IGenericRepository<Customer> genericRepository)
    {
        _customerRepository = customerRepository;
        _genericRepository = genericRepository;
    }

    public async Task<ActionResponse<CustomerDTO>> GetByIdAsync(int id)
    {
        var response = await _genericRepository.GetAsync(id);
        if (!response.WasSuccess)
        {
            return new ActionResponse<CustomerDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<CustomerDTO>
        {
            WasSuccess = true,
            Result = MapToDTO(response.Result!)
        };
    }

    public async Task<ActionResponse<IEnumerable<CustomerDTO>>> GetAllAsync()
    {
        // Retornar solo clientes activos según US-CUST-1
        var response = await _customerRepository.GetPaginatedAsync(1, int.MaxValue, isActive: true);
        if (!response.WasSuccess)
        {
            return new ActionResponse<IEnumerable<CustomerDTO>>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        var dtos = response.Result!.Items.Select(MapToDTO);
        return new ActionResponse<IEnumerable<CustomerDTO>>
        {
            WasSuccess = true,
            Result = dtos
        };
    }

    public async Task<ActionResponse<PaginationDTO<CustomerDTO>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null)
    {
        var response = await _customerRepository.GetPaginatedAsync(page, pageSize, isActive);
        if (!response.WasSuccess)
        {
            return new ActionResponse<PaginationDTO<CustomerDTO>>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        var pagination = response.Result!;
        var dtos = pagination.Items.Select(MapToDTO).ToList();

        return new ActionResponse<PaginationDTO<CustomerDTO>>
        {
            WasSuccess = true,
            Result = new PaginationDTO<CustomerDTO>
            {
                Items = dtos,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                Total = pagination.Total
            }
        };
    }

    public async Task<ActionResponse<CustomerDTO>> CreateAsync(CreateCustomerDTO dto)
    {
        var customer = new Customer
        {
            Name = dto.Name,
            TaxId = dto.TaxId,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var response = await _genericRepository.AddAsync(customer);
        if (!response.WasSuccess)
        {
            return new ActionResponse<CustomerDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<CustomerDTO>
        {
            WasSuccess = true,
            Result = MapToDTO(response.Result!)
        };
    }

    public async Task<ActionResponse<CustomerDTO>> UpdateAsync(int id, CustomerDTO dto)
    {
        if (id != dto.Id)
        {
            return new ActionResponse<CustomerDTO>
            {
                WasSuccess = false,
                Message = "El ID no coincide"
            };
        }

        var customer = new Customer
        {
            Id = dto.Id,
            Name = dto.Name,
            TaxId = dto.TaxId,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var response = await _genericRepository.UpdateAsync(customer);
        if (!response.WasSuccess)
        {
            return new ActionResponse<CustomerDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<CustomerDTO>
        {
            WasSuccess = true,
            Result = MapToDTO(response.Result!)
        };
    }

    public async Task<ActionResponse<bool>> DeleteAsync(int id)
    {
        var response = await _genericRepository.DeleteAsync(id);
        return new ActionResponse<bool>
        {
            WasSuccess = response.WasSuccess,
            Message = response.Message,
            Result = response.WasSuccess
        };
    }

    private CustomerDTO MapToDTO(Customer customer)
    {
        return new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            TaxId = customer.TaxId,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            IsActive = customer.IsActive
        };
    }
}
