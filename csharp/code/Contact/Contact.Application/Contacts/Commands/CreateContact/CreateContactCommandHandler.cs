using AutoMapper;
using Contact.Application.User;
using Contact.Domain.Entities;
using Contact.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using ContactDomainEntities = Contact.Domain.Entities.Contact;

namespace Contact.Application.Contacts.Commands.CreateContact;

public class CreateContactCommandHandler(ILogger<CreateContactCommandHandler> logger,
  IMapper mapper,
  CurrentUser currentUser,
  IContactsRepository contactsRepository,
  IPhonesRepository phonesRepository) : IRequestHandler<CreateContactCommand, int>
{
    public async Task<int> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(" 新增联系人 {@Contact}", request);
        var contact = mapper.Map<ContactDomainEntities>(request);
        var userId = currentUser.Id;
        contact.UserId = userId;
        int id = await contactsRepository.CreateAsync(contact);
        if (id > 0 && contact.Phones != null)
        {
            foreach (var Phone in contact.Phones)
            {
                Phone.UserId = userId;
                Phone.ContactId = id;
            }
            var newPhones = mapper.Map<List<Phone>>(contact.Phones);
            await phonesRepository.AddRangeAsync(newPhones);
        }
        return id;
    }
}