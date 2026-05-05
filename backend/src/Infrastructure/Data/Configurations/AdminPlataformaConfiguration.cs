using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AdminPlataformaConfiguration : IEntityTypeConfiguration<AdminPlataforma>
{
    public void Configure(EntityTypeBuilder<AdminPlataforma> builder)
    {
        builder.ToTable("admins_plataforma");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(a => a.Email)
            .IsUnique();

        builder.Property(a => a.PasswordHash)
            .IsRequired();
    }
}
