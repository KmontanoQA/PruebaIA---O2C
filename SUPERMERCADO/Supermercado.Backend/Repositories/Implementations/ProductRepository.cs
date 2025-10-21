using Microsoft.EntityFrameworkCore;
using Supermercado.Backend.Data;
using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Implementations;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly DataContext _context;

    public ProductRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<Product>> GetBySkuAsync(string sku)
    {
        try
        {
            var product = await _context.Products
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Sku == sku);

            if (product == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "Producto no encontrado"
                };
            }

            return new ActionResponse<Product>
            {
                WasSuccess = true,
                Result = product
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<Product>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<PaginationDTO<Product>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null)
    {
        try
        {
            var query = _context.Products
                .Include(p => p.Categoria)
                .AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ActionResponse<PaginationDTO<Product>>
            {
                WasSuccess = true,
                Result = new PaginationDTO<Product>
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
            return new ActionResponse<PaginationDTO<Product>>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<bool>> UpdateStockAsync(int productId, int qtyDelta)
    {
        try
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "Producto no encontrado"
                };
            }

            var newStock = product.StockQty + qtyDelta;
            if (newStock < 0)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = $"Stock insuficiente. Stock actual: {product.StockQty}, Requerido: {Math.Abs(qtyDelta)}"
                };
            }

            product.StockQty = newStock;
            await _context.SaveChangesAsync();

            return new ActionResponse<bool>
            {
                WasSuccess = true,
                Result = true
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<bool>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }
}
