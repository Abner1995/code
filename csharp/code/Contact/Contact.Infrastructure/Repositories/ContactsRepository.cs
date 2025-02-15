using Contact.Domain.Repositories;
using Contact.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ContactDomainEntities = Contact.Domain.Entities.Contact;

namespace Contact.Infrastructure.Repositories;

internal class ContactsRepository(ContactDbContexts contactDbContexts) : IContactsRepository
{
    public async Task<int> CreateAsync(ContactDomainEntities contact)
    {
        await contactDbContexts.Contacts.AddAsync(contact);
        await contactDbContexts.SaveChangesAsync();
        return contact.Id;
    }

    public async Task DeleteAsync(ContactDomainEntities contact)
    {
        contactDbContexts.Contacts.Remove(contact);
        await contactDbContexts.SaveChangesAsync();
    }

    public async Task<ContactDomainEntities?> GetByIdAsync(int id)
    {
        return await contactDbContexts.Contacts
            .Include(c => c.Phones)
            .FirstOrDefaultAsync(x=>x.Id.Equals(id));
    }

    public async Task UpdateAsync(ContactDomainEntities contact)
    {
        contactDbContexts.Contacts.Update(contact);
        await contactDbContexts.SaveChangesAsync();
    }
}
