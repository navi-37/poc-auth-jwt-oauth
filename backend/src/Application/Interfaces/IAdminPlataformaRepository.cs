using Domain.Entities;

namespace Application.Interfaces;

public interface IAdminPlataformaRepository
{
    Task<AdminPlataforma?> GetByEmailAsync(string email);
    Task<bool> AnyAsync();
    Task AddAsync(AdminPlataforma admin);
}
