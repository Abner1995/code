using Contact.Domain.Constants;
using Contact.Domain.Repositories;
using Contact.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ContactDomainEntities = Contact.Domain.Entities.Contact;

namespace Contact.Infrastructure.Repositories;

internal class ContactsRepository(ContactDbContexts dbContexts) : IContactsRepository
{
    public async Task<int> CreateAsync(ContactDomainEntities contact)
    {
        await dbContexts.Contacts.AddAsync(contact);
        await dbContexts.SaveChangesAsync();
        return contact.Id;
    }

    public async Task DeleteAsync(ContactDomainEntities contact)
    {
        dbContexts.Contacts.Remove(contact);
        await dbContexts.SaveChangesAsync();
    }

    public async Task<(IEnumerable<ContactDomainEntities>, int)> GetAllMatchingAsync(string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection sortDirection)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = dbContexts
            .Contacts
            .Where(r => searchPhraseLower == null || (r.UserName.ToLower().Contains(searchPhraseLower)));

        var totalCount = await baseQuery.CountAsync();

        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<ContactDomainEntities, object>>>
            {
                { nameof(ContactDomainEntities.Id), r => r.Id },
                { nameof(ContactDomainEntities.UserName), r => r.UserName },
            };

            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var restaurants = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (restaurants, totalCount);
    }

    public async Task<ContactDomainEntities?> GetByIdAsync(int id)
    {
        var contact = await dbContexts.Contacts
            //.Include(c => c.Phones)
            .FirstOrDefaultAsync(x=>x.Id.Equals(id));
        if(contact != null)
        {
            contact.Phones = await dbContexts.Phones.Where(x => x.ContactId == id).ToListAsync();
        }
        return contact;
    }

    public async Task UpdateAsync(ContactDomainEntities contact)
    {
        dbContexts.Contacts.Update(contact);
        await dbContexts.SaveChangesAsync();
    }
}
