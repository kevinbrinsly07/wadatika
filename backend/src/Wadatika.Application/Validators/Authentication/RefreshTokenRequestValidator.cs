using FluentValidation;
using Wadatika.Application.DTOs.Authentication;

namespace Wadatika.Application.Validators.Authentication;

public sealed class RefreshTokenRequestValidator
    : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
