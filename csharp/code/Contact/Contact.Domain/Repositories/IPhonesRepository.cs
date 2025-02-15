using Contact.Domain.Entities;

namespace Contact.Domain.Repositories;

public interface IPhonesRepository
{
    Task AddAsync(Phone phone);
    Task AddRangeAsync(List<Phone> phones);
    Task<List<Phone>?> GetByContactIdAsync(int contactId);
    Task DeleteRangeAsync(List<Phone> phones);
    Task UpdateAsync(Phone phone);
}
