using DTOs.Postingan;
using FluentValidation;

namespace Validators;

public class UpdatePostinganDtoValidator : AbstractValidator<UpdatePostinganDto>
{
    public UpdatePostinganDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Content)
            .NotEmpty();
    }
}
