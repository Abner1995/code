using FluentValidation;

namespace Contact.Application.Contacts.Commands.UpdateContact;

public class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactCommandValidator()
    {
        RuleFor(x => x.UserName)
            .Length(1, 10)
            .WithMessage("请添加姓名");
    }
}
