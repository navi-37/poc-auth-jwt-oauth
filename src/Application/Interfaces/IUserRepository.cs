using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, Guid tenantId);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
}
