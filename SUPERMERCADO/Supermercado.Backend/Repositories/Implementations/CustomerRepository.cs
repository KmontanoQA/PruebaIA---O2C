using Microsoft.EntityFrameworkCore;
using Supermercado.Backend.Data;
using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Implementations;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    private readonly DataContext _context;

    public CustomerRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<Customer>> GetByTaxIdAsync(string taxId)
    {
        try
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.TaxId == taxId);

            if (customer == null)
            {
                return new ActionResponse<Customer>
                {
                    WasSuccess = false,
                    Message = "Cliente no encontrado"
                };
            }

            return new ActionResponse<Customer>
            {
                WasSuccess = true,
                Result = customer
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<Customer>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<PaginationDTO<Customer>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null)
    {
        try
        {
            var query = _context.Customers.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ActionResponse<PaginationDTO<Customer>>
            {
                WasSuccess = true,
                Result = new PaginationDTO<Customer>
                {
                    Items = items,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                }
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<PaginationDTO<Customer>>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }
}
