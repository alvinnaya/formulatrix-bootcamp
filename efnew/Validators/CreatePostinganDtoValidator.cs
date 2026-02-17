using DTOs.Postingan;
using FluentValidation;

namespace Validators;

public class CreatePostinganDtoValidator : AbstractValidator<CreatePostinganDto>
{
    public CreatePostinganDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Content)
            .NotEmpty();
    }
}
