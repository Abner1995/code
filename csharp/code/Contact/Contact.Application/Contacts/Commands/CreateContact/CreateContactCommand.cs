using Contact.Application.Phones.Dtos;
using MediatR;

namespace Contact.Application.Contacts.Commands.CreateContact;

public class CreateContactCommand: IRequest<int>
{
    public string UserName { get; set; }
    public List<PhoneDto>? Phones { get; set; } = [];
}
