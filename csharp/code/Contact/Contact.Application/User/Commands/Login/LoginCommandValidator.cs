using FluentValidation;

namespace Contact.Application.User.Commands.Login;

public class LoginCommandValidator:AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserName)
            .Length(1, 10)
            .WithMessage("请输入用户名");
        RuleFor(x => x.PassWord)
            .Length(1, 10)
            .WithMessage("请输入密码");
    }
}
