using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Backend.UnitsOfWork.Interfaces;
using Supermercado.Shared.DTOs;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.UnitsOfWork.Implementations;

public class AuthUnitOfWork : IAuthUnitOfWork
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthUnitOfWork(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<ActionResponse<TokenDTO>> LoginAsync(LoginDTO dto)
    {
        try
        {
            var userResponse = await _userRepository.GetByEmailAsync(dto.Email);
            if (!userResponse.WasSuccess)
            {
                return new ActionResponse<TokenDTO>
                {
                    WasSuccess = false,
                    Message = "Credenciales inválidas"
                };
            }

            var user = userResponse.Result!;

            if (!user.IsActive)
            {
                return new ActionResponse<TokenDTO>
                {
                    WasSuccess = false,
                    Message = "Usuario inactivo"
                };
            }

            var isValidPassword = await _userRepository.ValidatePasswordAsync(user, dto.Password);
            if (!isValidPassword)
            {
                return new ActionResponse<TokenDTO>
                {
                    WasSuccess = false,
                    Message = "Credenciales inválidas"
                };
            }

            var token = GenerateJwtToken(user.Email, user.Role);
            var expiresIn = 3600; // 1 hora

            return new ActionResponse<TokenDTO>
            {
                WasSuccess = true,
                Result = new TokenDTO
                {
                    AccessToken = token,
                    ExpiresIn = expiresIn,
                    Email = user.Email,
                    Role = user.Role
                }
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<TokenDTO>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    private string GenerateJwtToken(string email, string role)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "SuperSecretKey_ChangeInProduction_MinLength32Characters!";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "SupermercadoAPI";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "SupermercadoClient";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
