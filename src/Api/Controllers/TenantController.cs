using Application.DTOs.Tenants;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("tenants")]
public class TenantController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tenants = await _tenantService.GetAllAsync();
        return Ok(tenants);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTenantRequest request)
    {
        var result = await _tenantService.CreateAsync(request);
        if (result is null)
            return Conflict(new { error = "A tenant with that slug already exists." });

        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _tenantService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
