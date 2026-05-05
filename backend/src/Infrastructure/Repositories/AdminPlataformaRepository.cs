using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AdminPlataformaRepository : IAdminPlataformaRepository
{
    private readonly AppDbContext _context;

    public AdminPlataformaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdminPlataforma?> GetByEmailAsync(string email)
    {
        return await _context.AdminsPlataforma.FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<bool> AnyAsync()
    {
        return await _context.AdminsPlataforma.AnyAsync();
    }

    public async Task AddAsync(AdminPlataforma admin)
    {
        await _context.AdminsPlataforma.AddAsync(admin);
        await _context.SaveChangesAsync();
    }
}
