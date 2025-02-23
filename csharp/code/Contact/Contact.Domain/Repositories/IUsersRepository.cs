using Contact.Domain.Entities;

namespace Contact.Domain.Repositories;

public interface IUsersRepository
{
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetByIdAsync(int userId);
    Task<User> AddAsync(User user);
    Task<bool> UserNameExistsAsync(string userName);
}
