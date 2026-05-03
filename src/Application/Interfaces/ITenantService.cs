using Application.DTOs.Tenants;

namespace Application.Interfaces;

public interface ITenantService
{
    Task<List<TenantResponse>> GetAllAsync();
    Task<TenantResponse?> CreateAsync(CreateTenantRequest request);
    Task<bool> DeleteAsync(Guid id);
}
