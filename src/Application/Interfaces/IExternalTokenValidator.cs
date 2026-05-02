using Application.DTOs.Auth;

namespace Application.Interfaces;

public interface IExternalTokenValidator
{
    string Provider { get; }
    Task<ExternalUserInfo?> ValidateAsync(string token);
}
