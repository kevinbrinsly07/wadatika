using FluentValidation;
using Wadatika.Application.DTOs.Tasks;

namespace Wadatika.Application.Features.Tasks;

public sealed class AssignTaskRequestValidator
    : AbstractValidator<AssignTaskRequest>
{
    public AssignTaskRequestValidator()
    {
        RuleFor(request => request.AssigneeUserId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("Assignee ID cannot be an empty GUID.");
    }
}