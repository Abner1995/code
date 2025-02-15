using Contact.Domain.Entities;
using Contact.Domain.Repositories;
using Contact.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Contact.Infrastructure.Repositories;

internal class PhonesRepository(ContactDbContexts contactDbContexts) : IPhonesRepository
{
    public async Task AddAsync(Phone phone)
    {
        await contactDbContexts.Phones.AddAsync(phone);
        await contactDbContexts.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<Phone> phones)
    {
        await contactDbContexts.Phones.AddRangeAsync(phones);
        await contactDbContexts.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(List<Phone> phones)
    {
        contactDbContexts.Phones.RemoveRange(phones);
        await contactDbContexts.SaveChangesAsync();
    }

    public async Task<List<Phone>?> GetByContactIdAsync(int contactId)
    {
        return await contactDbContexts.Phones.Where(p=>p.ContactId.Equals(contactId)).ToListAsync();
    }

    public async Task UpdateAsync(Phone phone)
    {
        contactDbContexts.Phones.Update(phone);
        await contactDbContexts.SaveChangesAsync();
    }
}
