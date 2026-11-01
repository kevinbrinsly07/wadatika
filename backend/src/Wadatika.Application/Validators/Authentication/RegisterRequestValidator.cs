using FluentValidation;
using Wadatika.Application.DTOs.Authentication;

namespace Wadatika.Application.Validators.Authentication;

public sealed class RegisterRequestValidator
     : AbstractValidator<RegisterRequest>

{
     public RegisterRequestValidator()
     {
          RuleFor(x => x.Email)
               .NotEmpty()
               .EmailAddress()
               .MaximumLength(256);

          RuleFor(x => x.Password)
               .NotEmpty()
               .MinimumLength(8)
               .Matches("[A-Z]")
               .WithMessage("Password must contain at least one uppercase letter.")
               .Matches("[a-z]")
               .WithMessage("Password must contain at least one lowercase letter.")
               .Matches("[0-9]")
               .WithMessage("Password must contain at least one digit.")
               .Matches("[^a-zA-Z0-9]")
               .WithMessage("Password must contain at least one special character.");

          RuleFor(x => x.ConfirmPassword)
          .Equal(x => x.Password)
          .WithMessage("Password do not match");


     }
}