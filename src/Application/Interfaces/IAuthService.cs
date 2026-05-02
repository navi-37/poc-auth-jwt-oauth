using Application.DTOs.Auth;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RefreshAsync(RefreshRequest request);
    Task<bool> LogoutAsync(LogoutRequest request);
}
