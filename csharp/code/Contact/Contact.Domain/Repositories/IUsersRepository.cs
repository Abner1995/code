using Contact.Domain.Entities;

namespace Contact.Domain.Repositories;

public interface IUsersRepository
{
    Task<User?> GetByUserNameAsync(string userName);
}
