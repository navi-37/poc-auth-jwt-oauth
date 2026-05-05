using System.Net.Http.Headers;
using System.Text.Json;
using Application.DTOs.Auth;
using Application.Interfaces;

namespace Infrastructure.Services;

public class GitHubTokenValidator : IExternalTokenValidator
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GitHubTokenValidator(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public string Provider => "github";

    public async Task<ExternalUserInfo?> ValidateAsync(string token)
    {
        var client = _httpClientFactory.CreateClient("GitHub");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("https://api.github.com/user");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var providerId = root.GetProperty("id").GetInt64().ToString();

        string? email = null;
        if (root.TryGetProperty("email", out var emailProp) && emailProp.ValueKind != JsonValueKind.Null)
            email = emailProp.GetString();

        if (email is null)
            email = await FetchPrimaryEmailAsync(client);

        if (email is null)
            return null;

        return new ExternalUserInfo(providerId, email);
    }

    private static async Task<string?> FetchPrimaryEmailAsync(HttpClient client)
    {
        var response = await client.GetAsync("https://api.github.com/user/emails");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement.EnumerateArray()
            .Where(e => e.GetProperty("primary").GetBoolean() && e.GetProperty("verified").GetBoolean())
            .Select(e => e.GetProperty("email").GetString())
            .FirstOrDefault();
    }
}
