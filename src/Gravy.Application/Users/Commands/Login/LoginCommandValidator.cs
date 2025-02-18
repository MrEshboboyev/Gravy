using FluentValidation;

namespace Gravy.Application.Users.Commands.Login;

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(user => user.Email).NotEmpty();

        RuleFor(user => user.Password).MinimumLength(5);
    }
}