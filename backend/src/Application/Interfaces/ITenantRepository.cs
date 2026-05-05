using Domain.Entities;

namespace Application.Interfaces;

public interface ITenantRepository
{
    Task<Tenant?> GetBySlugAsync(string slug);
    Task<Tenant?> GetByIdAsync(Guid id);
    Task<List<Tenant>> GetAllAsync();
    Task AddAsync(Tenant tenant);
    Task DeleteAsync(Tenant tenant);
}
