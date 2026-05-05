using Application.DTOs.Tenants;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;

    public TenantService(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<List<TenantResponse>> GetAllAsync()
    {
        var tenants = await _tenantRepository.GetAllAsync();
        return tenants.Select(t => new TenantResponse(t.Id, t.Slug, t.Name, t.CreatedAt)).ToList();
    }

    public async Task<TenantResponse?> CreateAsync(CreateTenantRequest request)
    {
        var existing = await _tenantRepository.GetBySlugAsync(request.Slug);
        if (existing is not null)
            return null;

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Slug = request.Slug.ToLowerInvariant(),
            Name = request.Name
        };

        await _tenantRepository.AddAsync(tenant);
        return new TenantResponse(tenant.Id, tenant.Slug, tenant.Name, tenant.CreatedAt);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        if (tenant is null)
            return false;

        await _tenantRepository.DeleteAsync(tenant);
        return true;
    }
}
