using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/account/login";
        options.LogoutPath = "/account/logout";
        options.AccessDeniedPath = "/account/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Repositories
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IAdminPlataformaRepository, AdminPlataformaRepository>();

// Services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IAdminPlataformaService, AdminPlataformaService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        await SeedDefaultAdminAsync(scope.ServiceProvider, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error durante el seed del admin por defecto.");
        throw;
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/home/error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tenant}/{action=Index}/{id?}");

app.Run();

static async Task SeedDefaultAdminAsync(IServiceProvider services, ILogger logger)
{
    var repo = services.GetRequiredService<IAdminPlataformaRepository>();
    if (await repo.AnyAsync())
    {
        logger.LogInformation("Admin ya existe, seed omitido.");
        return;
    }

    var hasher = services.GetRequiredService<IPasswordHasher>();
    var config = services.GetRequiredService<IConfiguration>();

    var email = config["Seed:AdminEmail"];
    if (string.IsNullOrEmpty(email)) email = "admin@tupenca.uy";

    var password = config["Seed:AdminPassword"];
    if (string.IsNullOrEmpty(password)) password = "Admin1234!";

    logger.LogInformation("Creando admin por defecto: {Email}", email);

    await repo.AddAsync(new AdminPlataforma
    {
        Id = Guid.NewGuid(),
        Email = email,
        PasswordHash = hasher.Hash(password)
    });

    logger.LogInformation("Admin creado exitosamente.");
}
