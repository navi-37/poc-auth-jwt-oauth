using Application.Interfaces;

namespace Api.Services;

public class TenantContext : ITenantContext
{
    public Guid TenantId { get; set; }
}
