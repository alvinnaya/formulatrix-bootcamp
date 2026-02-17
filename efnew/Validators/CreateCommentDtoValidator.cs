using DTOs.Comment;
using FluentValidation;

namespace Validators;

public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(x => x.Isi)
            .NotEmpty()
            .MinimumLength(1);
    }
}
