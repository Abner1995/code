using AutoMapper;
using Contact.Domain.Entities;
using Contact.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using ContactDomainEntities = Contact.Domain.Entities.Contact;

namespace Contact.Application.Contacts.Commands.CreateContact;

public class CreateContactCommandHandler(ILogger<CreateContactCommandHandler> logger,
  IMapper mapper,
  IContactsRepository contactsRepository) : IRequestHandler<CreateContactCommand, int>
{
    public async Task<int> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(" 新增联系人 {@Contact}", request);
        var contact = mapper.Map<ContactDomainEntities>(request);
        int id = await contactsRepository.CreateAsync(contact);
        return id;
    }
}
