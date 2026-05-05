using Application.DTOs.Tenants;
using Application.Interfaces;
using Backoffice.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backoffice.Controllers;

[Authorize]
public class TenantController : Controller
{
    private readonly ITenantService _tenantService;

    public TenantController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tenants = await _tenantService.GetAllAsync();
        return View(tenants);
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateTenantViewModel());

    [HttpPost]
    public async Task<IActionResult> Create(CreateTenantViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _tenantService.CreateAsync(new CreateTenantRequest(model.Slug, model.Name));
        if (result is null)
        {
            ModelState.AddModelError(nameof(model.Slug), "Ya existe un tenant con ese slug.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _tenantService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
