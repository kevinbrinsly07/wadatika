using FluentValidation;
using Wadatika.Application.DTOs.Tasks;

namespace Wadatika.Application.Features.Tasks;

public sealed class UpdateTaskStatusRequestValidator
    : AbstractValidator<UpdateTaskStatusRequest>
{
    public UpdateTaskStatusRequestValidator()
    {
        RuleFor(request => request.Status)
            .IsInEnum();
    }
}