using Domain.Entities;

namespace Application.Interfaces;

public interface IExternalLoginRepository
{
    Task<ExternalLogin?> GetAsync(string provider, string providerUserId);
    Task AddAsync(ExternalLogin externalLogin);
}
