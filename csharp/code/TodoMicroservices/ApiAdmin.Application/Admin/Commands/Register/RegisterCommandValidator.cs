using FluentValidation;

namespace ApiAdmin.Application.Admin.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.UserName)
            .Length(1, 10)
            .WithMessage("请输入用户名");
        RuleFor(x => x.PassWord)
            .Length(1, 10)
            .WithMessage("请输入密码");
    }
}
