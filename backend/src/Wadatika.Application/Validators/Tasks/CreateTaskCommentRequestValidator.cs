using FluentValidation;
using Wadatika.Application.DTOs.Tasks;

namespace Wadatika.Application.Validators.Tasks;

public sealed class CreateTaskCommentRequestValidator
    : AbstractValidator<CreateTaskCommentRequest>
{
    public CreateTaskCommentRequestValidator()
    {
        RuleFor(request => request.Content)
            .NotEmpty()
            .MaximumLength(5000);
    }
}