using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.UnitsOfWork.Implementations;

public class SellerUnitOfWork : ISellerUnitOfWork
{
    private readonly IGenericRepository<Seller> _genericRepository;
    private readonly ISellerRepository _sellerRepository;

    public SellerUnitOfWork(IGenericRepository<Seller> genericRepository, ISellerRepository sellerRepository)
    {
        _genericRepository = genericRepository;
        _sellerRepository = sellerRepository;
    }

    public async Task<ActionResponse<SellerDTO>> GetByIdAsync(int id)
    {
        var response = await _genericRepository.GetAsync(id);
        if (!response.WasSuccess)
        {
            return new ActionResponse<SellerDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<SellerDTO>
        {
            WasSuccess = true,
            Result = MapToDTO(response.Result!)
        };
    }

    public async Task<ActionResponse<IEnumerable<SellerDTO>>> GetAllAsync()
    {
        // Retornar solo vendedores activos
        var response = await _sellerRepository.GetPaginatedAsync(1, int.MaxValue, isActive: true);
        if (!response.WasSuccess)
        {
            return new ActionResponse<IEnumerable<SellerDTO>>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        var dtos = response.Result!.Items.Select(MapToDTO);
        return new ActionResponse<IEnumerable<SellerDTO>>
        {
            WasSuccess = true,
            Result = dtos
        };
    }

    public async Task<ActionResponse<PaginationDTO<SellerDTO>>> GetPaginatedAsync(int page, int pageSize, bool? isActive = null)
    {
        var response = await _sellerRepository.GetPaginatedAsync(page, pageSize, isActive);
        if (!response.WasSuccess)
        {
            return new ActionResponse<PaginationDTO<SellerDTO>>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        var pagination = response.Result!;
        var dtos = pagination.Items.Select(MapToDTO).ToList();

        return new ActionResponse<PaginationDTO<SellerDTO>>
        {
            WasSuccess = true,
            Result = new PaginationDTO<SellerDTO>
            {
                Items = dtos,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                Total = pagination.Total
            }
        };
    }

    public async Task<ActionResponse<SellerDTO>> CreateAsync(CreateSellerDTO dto)
    {
        // Validar que el código no exista
        var existingCode = await _sellerRepository.GetByCodeAsync(dto.Code);
        if (existingCode.WasSuccess)
        {
            return new ActionResponse<SellerDTO>
            {
                WasSuccess = false,
                Message = $"Ya existe un vendedor con el código {dto.Code}"
            };
        }

        var seller = new Seller
        {
            FullName = dto.FullName,
            Code = dto.Code,
            Email = dto.Email,
            Phone = dto.Phone,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var response = await _genericRepository.AddAsync(seller);
        if (!response.WasSuccess)
        {
            return new ActionResponse<SellerDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<SellerDTO>
        {
            WasSuccess = true,
            Result = MapToDTO(response.Result!)
        };
    }

    public async Task<ActionResponse<SellerDTO>> UpdateAsync(int id, SellerDTO dto)
    {
        var sellerResponse = await _genericRepository.GetAsync(id);
        if (!sellerResponse.WasSuccess)
        {
            return new ActionResponse<SellerDTO>
            {
                WasSuccess = false,
                Message = "Vendedor no encontrado"
            };
        }

        var seller = sellerResponse.Result!;

        // Validar código único si cambió
        if (seller.Code != dto.Code)
        {
            var existingCode = await _sellerRepository.GetByCodeAsync(dto.Code);
            if (existingCode.WasSuccess)
            {
                return new ActionResponse<SellerDTO>
                {
                    WasSuccess = false,
                    Message = $"Ya existe un vendedor con el código {dto.Code}"
                };
            }
        }

        seller.FullName = dto.FullName;
        seller.Code = dto.Code;
        seller.Email = dto.Email;
        seller.Phone = dto.Phone;
        seller.IsActive = dto.IsActive;

        var response = await _genericRepository.UpdateAsync(seller);
        if (!response.WasSuccess)
        {
            return new ActionResponse<SellerDTO>
            {
                WasSuccess = false,
                Message = response.Message
            };
        }

        return new ActionResponse<SellerDTO>
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

    private static SellerDTO MapToDTO(Seller seller)
    {
        return new SellerDTO
        {
            Id = seller.Id,
            FullName = seller.FullName,
            Code = seller.Code,
            Email = seller.Email,
            Phone = seller.Phone,
            IsActive = seller.IsActive,
            CreatedAt = seller.CreatedAt
        };
    }
}
