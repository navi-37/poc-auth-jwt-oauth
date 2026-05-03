using Api.Services;
using Application.Interfaces;

namespace Api.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _baseDomain;
    private readonly string _tenantHeader;

    public TenantResolutionMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _baseDomain = configuration["Multitenancy:BaseDomain"] ?? "tupenca.uy";
        _tenantHeader = configuration["Multitenancy:TenantHeader"] ?? "X-Tenant";
    }

    public async Task InvokeAsync(HttpContext context, ITenantRepository tenantRepository, TenantContext tenantContext)
    {
        if (context.Request.Path.StartsWithSegments("/tenants"))
        {
            await _next(context);
            return;
        }

        var slug = ResolveSlug(context);

        if (slug is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant not specified." });
            return;
        }

        var tenant = await tenantRepository.GetBySlugAsync(slug);

        if (tenant is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant not found." });
            return;
        }

        tenantContext.TenantId = tenant.Id;
        await _next(context);
    }

    private string? ResolveSlug(HttpContext context)
    {
        var host = context.Request.Host.Host;

        if (host.EndsWith($".{_baseDomain}", StringComparison.OrdinalIgnoreCase))
        {
            var slug = host[..^(_baseDomain.Length + 1)];
            if (!string.IsNullOrEmpty(slug))
                return slug;
        }

        if (context.Request.Headers.TryGetValue(_tenantHeader, out var headerValue))
        {
            var slug = headerValue.ToString();
            if (!string.IsNullOrEmpty(slug))
                return slug;
        }

        return null;
    }
}
