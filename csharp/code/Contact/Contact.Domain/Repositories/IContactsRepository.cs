using ContactDomainEntities = Contact.Domain.Entities.Contact;

namespace Contact.Domain.Repositories;

public interface IContactsRepository
{
    Task<int> CreateAsync(ContactDomainEntities contact);
    Task<ContactDomainEntities?> GetByIdAsync(int id);
    Task UpdateAsync(ContactDomainEntities contact);
    Task DeleteAsync(ContactDomainEntities contact);
}
