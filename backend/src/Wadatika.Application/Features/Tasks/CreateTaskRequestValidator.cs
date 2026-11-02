using FluentValidation;
using Wadatika.Application.DTOs.Tasks;

namespace Wadatika.Application.Features.Tasks;

public sealed class CreateTaskRequestValidator
    : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Description)
            .MaximumLength(2000);

        RuleFor(request => request.DueDate)
            .Must(date => !date.HasValue || date.Value >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Due date cannot be in the past.");
    }
}