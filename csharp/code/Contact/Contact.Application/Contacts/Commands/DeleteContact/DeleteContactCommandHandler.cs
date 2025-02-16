using Contact.Domain.Exceptions;
using Contact.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using ContactDomainEntities = Contact.Domain.Entities.Contact;

namespace Contact.Application.Contacts.Commands.DeleteContact;

public class DeleteContactCommandHandler(ILogger<DeleteContactCommandHandler> logger,
    IContactsRepository contactsRepository) : IRequestHandler<DeleteContactCommand>
{
    public async Task Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("删除联系人 with id: {ContactId}", request.Id);

        var contact = await contactsRepository.GetByIdAsync(request.Id);
        if (contact is null)
            throw new NotFoundException(nameof(ContactDomainEntities), request.Id.ToString());

        await contactsRepository.DeleteAsync(contact);
    }
}