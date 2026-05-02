using Application.DTOs.Auth;

namespace Application.Interfaces;

public interface IOAuthService
{
    Task<LoginResponse?> LoginAsync(string provider, string token);
}
