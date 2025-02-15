using FluentValidation;

namespace Contact.Application.Contacts.Commands.CreateContact;

public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
    public CreateContactCommandValidator()
    {
        RuleFor(x=>x.UserName)
            .Length(1,10)
            .WithMessage("请添加姓名");
    }
}
