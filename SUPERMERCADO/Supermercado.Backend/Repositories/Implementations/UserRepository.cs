using Microsoft.EntityFrameworkCore;
using Supermercado.Backend.Data;
using Supermercado.Backend.Repositories.Interfaces;
using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<ActionResponse<User>> GetByEmailAsync(string email)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                return new ActionResponse<User>
                {
                    WasSuccess = false,
                    Message = "Usuario no encontrado"
                };
            }

            return new ActionResponse<User>
            {
                WasSuccess = true,
                Result = user
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<User>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ActionResponse<User>> CreateUserAsync(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new ActionResponse<User>
            {
                WasSuccess = true,
                Result = user
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<User>
            {
                WasSuccess = false,
                Message = "Ya existe un usuario con ese email"
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<User>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public Task<bool> ValidatePasswordAsync(User user, string password)
    {
        // En producción usar BCrypt.Net-Next o similar
        return Task.FromResult(user.PasswordHash == HashPassword(password));
    }

    public static string HashPassword(string password)
    {
        // NOTA: En producción usar BCrypt.Net-Next
        // Por ahora usamos un hash simple para demostración
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
