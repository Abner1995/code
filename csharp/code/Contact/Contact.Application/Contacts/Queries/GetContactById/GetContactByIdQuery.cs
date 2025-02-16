using Contact.Application.Contacts.Dtos;
using MediatR;

namespace Contact.Application.Contacts.Queries.GetContactById;

public class GetContactByIdQuery(int id) : IRequest<ContactDto>
{
    public int Id { get; } = id;
}
