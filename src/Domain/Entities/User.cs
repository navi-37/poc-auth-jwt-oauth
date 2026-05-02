namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<ExternalLogin> ExternalLogins { get; set; } = [];
}
