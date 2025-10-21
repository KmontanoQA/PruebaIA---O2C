using Supermercado.Shared.Entities;
using Supermercado.Shared.Responses;

namespace Supermercado.Backend.Repositories.Interfaces;

public interface IUserRepository
{
    Task<ActionResponse<User>> GetByEmailAsync(string email);
    Task<ActionResponse<User>> CreateUserAsync(User user);
    Task<bool> ValidatePasswordAsync(User user, string password);
}
