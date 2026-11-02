using FluentValidation;
using Wadatika.Application.DTOs.Teams;

namespace Wadatika.Application.Validators.Teams;

public sealed class CreateTeamRequestValidator
    : AbstractValidator<CreateTeamRequest>
{
    public CreateTeamRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Team name is required.")
            .MaximumLength(100)
            .WithMessage("Team name cannot exceed 100 characters.");
    }
}