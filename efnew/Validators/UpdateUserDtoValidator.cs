using DTOs.User;
using FluentValidation;

namespace Validators;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3);
    }
}
