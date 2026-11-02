using FluentValidation;
using Wadatika.Application.DTOs.Tasks;

namespace Wadatika.Application.Features.Tasks;

public sealed class UpdateTaskPriorityRequestValidator
    : AbstractValidator<UpdateTaskPriorityRequest>
{
    public UpdateTaskPriorityRequestValidator()
    {
        RuleFor(request => request.Priority)
            .IsInEnum();
    }
}