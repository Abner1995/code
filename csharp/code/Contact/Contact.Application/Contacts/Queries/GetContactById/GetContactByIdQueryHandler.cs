using AutoMapper;
using Contact.Application.Contacts.Dtos;
using Contact.Domain.Exceptions;
using Contact.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using ContactDomainEntities = Contact.Domain.Entities.Contact;

namespace Contact.Application.Contacts.Queries.GetContactById;

public class GetContactByIdQueryHandler(ILogger<GetContactByIdQueryHandler> logger,
    IMapper mapper,
    IContactsRepository contactsRepository) : IRequestHandler<GetContactByIdQuery, ContactDto>
{
    public async Task<ContactDto> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting contact {ContactId}", request.Id);
        var contact = await contactsRepository.GetByIdAsync(request.Id)
               ?? throw new NotFoundException(nameof(ContactDomainEntities), request.Id.ToString());
        var contactDto = mapper.Map<ContactDto>(contact);
        return contactDto;
    }
}
