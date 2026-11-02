using FluentValidation;
using Wadatika.Application.DTOs.Projects;

namespace Wadatika.Application.Validators.Projects;

public sealed class CreateProjectRequestValidator
    : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Project name is required.")
            .MaximumLength(100)
            .WithMessage("Project name cannot exceed 100 characters.");

        RuleFor(request => request.Key)
            .NotEmpty()
            .WithMessage("Project key is required.")
            .MaximumLength(10)
            .WithMessage("Project key cannot exceed 10 characters.")
            .Matches("^[A-Za-z][A-Za-z0-9]{1,9}$")
            .WithMessage(
                "Project key must start with a letter and contain 2–10 letters or numbers.");

        RuleFor(request => request.Description)
            .MaximumLength(500)
            .WithMessage("Project description cannot exceed 500 characters.");
    }
}