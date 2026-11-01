using FluentValidation;
using Wadatika.Application.DTOs.Authentication;

namespace Wadatika.Application.Validators.Authentication;

public sealed class LoginRequestValidator
    : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}