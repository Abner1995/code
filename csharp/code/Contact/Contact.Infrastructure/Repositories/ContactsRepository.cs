using Contact.Domain.Constants;
using Contact.Domain.Entities;
using Contact.Domain.Repositories;
using Contact.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
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

    public async Task<(IEnumerable<ContactDomainEntities>, int)> GetAllMatchingAsync(string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection sortDirection)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = contactDbContexts
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
