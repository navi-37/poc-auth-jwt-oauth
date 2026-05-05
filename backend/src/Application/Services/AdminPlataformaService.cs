using Application.DTOs.Backoffice;
using Application.Interfaces;

namespace Application.Services;

public class AdminPlataformaService : IAdminPlataformaService
{
    private readonly IAdminPlataformaRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public AdminPlataformaService(IAdminPlataformaRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AdminPlataformaDto?> ValidateAsync(string email, string password)
    {
        var admin = await _repository.GetByEmailAsync(email);
        if (admin is null || !_passwordHasher.Verify(password, admin.PasswordHash))
            return null;

        return new AdminPlataformaDto(admin.Id, admin.Email);
    }
}
