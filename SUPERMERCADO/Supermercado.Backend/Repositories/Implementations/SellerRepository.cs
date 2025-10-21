using Microsoft.EntityFrameworkCore;
using Supermercado.Backend.Data;
using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Implementations;

public class SellerRepository : GenericRepository<Seller>, ISellerRepository
{
    private readonly DataContext _context;

    public SellerRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<Seller>> GetByCodeAsync(string code)
    {
        try
        {
            var seller = await _context.Sellers
                .FirstOrDefaultAsync(s => s.Code == code);

            if (seller == null)
            {
                return new ActionResponse<Seller>
                {
                    WasSuccess = false,
                    Message = "Vendedor no encontrado"
                };
            }

            return new ActionResponse<Seller>
            {
                WasSuccess = true,
                Result = seller
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<Seller>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<PaginationDTO<Seller>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null)
    {
        try
        {
            var query = _context.Sellers.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(s => s.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ActionResponse<PaginationDTO<Seller>>
            {
                WasSuccess = true,
                Result = new PaginationDTO<Seller>
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
            return new ActionResponse<PaginationDTO<Seller>>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }
}
