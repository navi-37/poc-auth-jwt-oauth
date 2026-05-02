using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
            return false;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        await _userRepository.AddAsync(user);
        return true;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null || user.PasswordHash is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return null;

        await _refreshTokenRepository.RevokeAllByUserIdAsync(user.Id);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new LoginResponse(accessToken, refreshTokenValue);
    }

    public async Task<LoginResponse?> RefreshAsync(RefreshRequest request)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (storedToken is null || storedToken.Revoked || storedToken.ExpiresAt <= DateTime.UtcNow)
            return null;

        await _refreshTokenRepository.RevokeAsync(storedToken);

        var user = await _userRepository.GetByIdAsync(storedToken.UserId);
        if (user is null)
            return null;

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken);

        return new LoginResponse(newAccessToken, newRefreshTokenValue);
    }

    public async Task<bool> LogoutAsync(LogoutRequest request)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (storedToken is null || storedToken.Revoked)
            return false;

        await _refreshTokenRepository.RevokeAsync(storedToken);
        return true;
    }
}
