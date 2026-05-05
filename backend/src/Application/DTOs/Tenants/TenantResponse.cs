namespace Application.DTOs.Tenants;

public record TenantResponse(Guid Id, string Slug, string Name, DateTime CreatedAt);
