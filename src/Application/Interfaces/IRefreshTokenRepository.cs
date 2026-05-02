using Domain.Entities;

namespace Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task AddAsync(RefreshToken refreshToken);
    Task RevokeAsync(RefreshToken refreshToken);
    Task RevokeAllByUserIdAsync(Guid userId);
}
