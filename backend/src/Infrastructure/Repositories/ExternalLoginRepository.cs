using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ExternalLoginRepository : IExternalLoginRepository
{
    private readonly AppDbContext _context;

    public ExternalLoginRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ExternalLogin?> GetAsync(string provider, string providerUserId, Guid tenantId)
    {
        return await _context.ExternalLogins
            .Include(e => e.User)
            .FirstOrDefaultAsync(e =>
                e.Provider == provider &&
                e.ProviderUserId == providerUserId &&
                e.User.TenantId == tenantId);
    }

    public async Task AddAsync(ExternalLogin externalLogin)
    {
        await _context.ExternalLogins.AddAsync(externalLogin);
        await _context.SaveChangesAsync();
    }
}
