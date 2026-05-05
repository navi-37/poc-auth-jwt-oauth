using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators;

public class OAuthLoginRequestValidator : AbstractValidator<OAuthLoginRequest>
{
    public OAuthLoginRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
    }
}
