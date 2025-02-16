using Contact.Domain.Constants;
using ContactDomainEntities = Contact.Domain.Entities.Contact;

namespace Contact.Domain.Repositories;

public interface IContactsRepository
{
    Task<int> CreateAsync(ContactDomainEntities contact);
    Task<ContactDomainEntities?> GetByIdAsync(int id);
    Task UpdateAsync(ContactDomainEntities contact);
    Task DeleteAsync(ContactDomainEntities contact);
    Task<(IEnumerable<ContactDomainEntities>, int)> GetAllMatchingAsync(string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection sortDirection);
}
