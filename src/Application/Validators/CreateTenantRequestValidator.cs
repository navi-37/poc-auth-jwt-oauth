using Application.DTOs.Tenants;
using FluentValidation;

namespace Application.Validators;

public class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantRequestValidator()
    {
        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100)
            .Matches("^[a-z0-9-]+$").WithMessage("Slug must contain only lowercase letters, numbers and hyphens.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256);
    }
}
