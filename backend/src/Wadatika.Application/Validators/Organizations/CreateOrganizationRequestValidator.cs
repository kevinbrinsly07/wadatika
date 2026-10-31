using System.Net.Cache;
using FluentValidation;
using Wadatika.Application.DTOs.Organizations;

namespace Wadatika.Application.Validators.Organizations;

public sealed class CreateOrganizationValidator
     : AbstractValidator<CreateOrganizationRequest>
{
     public CreateOrganizationValidator()
     {
          RuleFor(request => request.Name)
               .NotEmpty()
               .WithMessage("Organization name is required. ")
               .MaximumLength(100)
               .WithMessage("Organization name cannot exceed 100 characters. ");

          RuleFor(request => request.Slug)
               .NotEmpty()
               .WithMessage("Organization slug is requered. ")
               .MaximumLength(100)
               .WithMessage("Organization slug cannot exceed 100 characters. ")
               .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
               .WithMessage(
                    "Slug must contain lowercase letters, numbers and singel hyphens.");
     }
}