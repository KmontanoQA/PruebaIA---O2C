using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.UnitsOfWork.Implementations;

public class ProductUnitOfWork : IProductUnitOfWork
{
    private readonly IProductRepository _productRepository;
    private readonly IGenericRepository<Product> _genericRepository;

    public ProductUnitOfWork(IProductRepository productRepository, IGenericRepository<Product> genericRepository)
    {
        _productRepository = productRepository;
        _genericRepository = genericRepository;
    }

    public async Task<ActionResponse<ProductDTO>> GetByIdAsync(int id)
    {
        var response = await _genericRepository.GetAsync(id);
        if (!response.WasSuccess)
        {
            return new ActionResponse<ProductDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<ProductDTO>
        {
            WasSuccess = true,
            Result = MapToDTO(response.Result!)
        };
    }

    public async Task<ActionResponse<IEnumerable<ProductDTO>>> GetAllAsync()
    {
        var response = await _genericRepository.GetAsync();
        if (!response.WasSuccess)
        {
            return new ActionResponse<IEnumerable<ProductDTO>>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        var dtos = response.Result!.Select(MapToDTO);
        return new ActionResponse<IEnumerable<ProductDTO>>
        {
            WasSuccess = true,
            Result = dtos
        };
    }

    public async Task<ActionResponse<PaginationDTO<ProductDTO>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null)
    {
        var response = await _productRepository.GetPaginatedAsync(page, pageSize, isActive);
        if (!response.WasSuccess)
        {
            return new ActionResponse<PaginationDTO<ProductDTO>>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        var pagination = response.Result!;
        var dtos = pagination.Items.Select(MapToDTO).ToList();

        return new ActionResponse<PaginationDTO<ProductDTO>>
        {
            WasSuccess = true,
            Result = new PaginationDTO<ProductDTO>
            {
                Items = dtos,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                Total = pagination.Total
            }
        };
    }

    public async Task<ActionResponse<ProductDTO>> GetBySkuAsync(string sku)
    {
        var response = await _productRepository.GetBySkuAsync(sku);
        if (!response.WasSuccess)
        {
            return new ActionResponse<ProductDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<ProductDTO>
        {
            WasSuccess = true,
            Result = MapToDTO(response.Result!)
        };
    }

    public async Task<ActionResponse<ProductDTO>> CreateAsync(CreateProductDTO dto)
    {
        var product = new Product
        {
            Sku = dto.Sku,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            TaxRatePct = dto.TaxRatePct,
            StockQty = dto.StockQty,
            CategoriaId = dto.CategoriaId,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var response = await _genericRepository.AddAsync(product);
        if (!response.WasSuccess)
        {
            return new ActionResponse<ProductDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<ProductDTO>
        {
            WasSuccess = true,
            Result = MapToDTO(response.Result!)
        };
    }

    public async Task<ActionResponse<ProductDTO>> UpdateAsync(int id, ProductDTO dto)
    {
        if (id != dto.Id)
        {
            return new ActionResponse<ProductDTO>
            {
                WasSuccess = false,
                Message = "El ID no coincide"
            };
        }

        var product = new Product
        {
            Id = dto.Id,
            Sku = dto.Sku,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            TaxRatePct = dto.TaxRatePct,
            StockQty = dto.StockQty,
            CategoriaId = dto.CategoriaId,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var response = await _genericRepository.UpdateAsync(product);
        if (!response.WasSuccess)
        {
            return new ActionResponse<ProductDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<ProductDTO>
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

    private ProductDTO MapToDTO(Product product)
    {
        return new ProductDTO
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            TaxRatePct = product.TaxRatePct,
            StockQty = product.StockQty,
            CategoriaId = product.CategoriaId,
            CategoriaNombre = product.Categoria?.descripcion,
            IsActive = product.IsActive
        };
    }
}
