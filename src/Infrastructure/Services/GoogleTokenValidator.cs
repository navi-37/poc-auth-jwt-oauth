using Application.DTOs.Auth;
using Application.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class GoogleTokenValidator : IExternalTokenValidator
{
    private readonly string _clientId;

    public GoogleTokenValidator(IConfiguration configuration)
    {
        _clientId = configuration["OAuth:Google:ClientId"]!;
    }

    public string Provider => "google";

    public async Task<ExternalUserInfo?> ValidateAsync(string token)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_clientId]
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
            return new ExternalUserInfo(payload.Subject, payload.Email);
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}
