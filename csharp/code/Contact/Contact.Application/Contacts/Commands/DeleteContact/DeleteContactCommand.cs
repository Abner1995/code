using MediatR;

namespace Contact.Application.Contacts.Commands.DeleteContact;

public class DeleteContactCommand(int id): IRequest
{
    public int Id { get; } = id;
}
