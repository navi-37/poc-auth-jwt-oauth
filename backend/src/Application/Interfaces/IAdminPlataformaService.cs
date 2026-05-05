using Application.DTOs.Backoffice;

namespace Application.Interfaces;

public interface IAdminPlataformaService
{
    Task<AdminPlataformaDto?> ValidateAsync(string email, string password);
}
