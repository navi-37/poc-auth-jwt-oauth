using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class OAuthService : IOAuthService
{
    private readonly IEnumerable<IExternalTokenValidator> _validators;
    private readonly IUserRepository _userRepository;
    private readonly IExternalLoginRepository _externalLoginRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;

    public OAuthService(
        IEnumerable<IExternalTokenValidator> validators,
        IUserRepository userRepository,
        IExternalLoginRepository externalLoginRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService)
    {
        _validators = validators;
        _userRepository = userRepository;
        _externalLoginRepository = externalLoginRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse?> LoginAsync(string provider, string token)
    {
        var validator = _validators.FirstOrDefault(v => v.Provider == provider);
        if (validator is null)
            return null;

        var userInfo = await validator.ValidateAsync(token);
        if (userInfo is null)
            return null;

        var externalLogin = await _externalLoginRepository.GetAsync(provider, userInfo.ProviderUserId);

        User user;
        if (externalLogin is not null)
        {
            user = (await _userRepository.GetByIdAsync(externalLogin.UserId))!;
        }
        else
        {
            user = await _userRepository.GetByEmailAsync(userInfo.Email)
                   ?? new User { Id = Guid.NewGuid(), Email = userInfo.Email };

            if (await _userRepository.GetByIdAsync(user.Id) is null)
                await _userRepository.AddAsync(user);

            await _externalLoginRepository.AddAsync(new ExternalLogin
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Provider = provider,
                ProviderUserId = userInfo.ProviderUserId
            });
        }

        await _refreshTokenRepository.RevokeAllByUserIdAsync(user.Id);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false
        });

        return new LoginResponse(accessToken, refreshTokenValue);
    }
}
