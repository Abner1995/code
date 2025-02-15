using Contact.Application.Phones.Dtos;
using MediatR;

namespace Contact.Application.Contacts.Commands.UpdateContact;

public class UpdateContactCommand: IRequest
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public List<UpdatePhoneDto>? Phones { get; set; } = [];
}
