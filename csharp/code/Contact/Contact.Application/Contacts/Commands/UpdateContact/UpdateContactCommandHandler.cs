using AutoMapper;
using Contact.Domain.Entities;
using Contact.Domain.Exceptions;
using Contact.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using ContactDomainEntities = Contact.Domain.Entities.Contact;

namespace Contact.Application.Contacts.Commands.UpdateContact;

public class UpdateContactCommandHandler(ILogger<UpdateContactCommandHandler> logger,
    IMapper mapper,
    IContactsRepository contactsRepository,
    IPhonesRepository phonesRepository) : IRequestHandler<UpdateContactCommand>
{
    public async Task Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(" 修改联系人 {@Contact}", request);

        var contact = await contactsRepository.GetByIdAsync(request.Id);
        if (contact is null)
            throw new NotFoundException(nameof(ContactDomainEntities), request.Id.ToString());

        mapper.Map(request, contact);
        await contactsRepository.UpdateAsync(contact);

        if (request.Phones != null && request.Phones.Any())
        {
            var existingPhones = await phonesRepository.GetByContactIdAsync(request.Id);

            var phonesToDelete = existingPhones
                .Where(existingPhone => request.Phones.All(phoneDto => phoneDto.Id != existingPhone.Id))
                .ToList();
            await phonesRepository.DeleteRangeAsync(phonesToDelete);

            foreach (var phoneDto in request.Phones)
            {
                var existingPhone = existingPhones.FirstOrDefault(p => p.Id == phoneDto.Id);
                if (existingPhone != null)
                {
                    mapper.Map(phoneDto, existingPhone);
                    await phonesRepository.UpdateAsync(existingPhone);
                }
                else
                {
                    var newPhone = mapper.Map<Phone>(phoneDto);
                    newPhone.ContactId = request.Id;
                    await phonesRepository.AddAsync(newPhone);
                }
            }
        }
    }
}