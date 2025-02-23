using Contact.Domain.Entities;
using Contact.Domain.Repositories;
using Contact.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Contact.Infrastructure.Repositories;

internal class UsersRepository(ContactDbContexts dbContexts) : IUsersRepository
{
    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await dbContexts.Users.FirstOrDefaultAsync(x=>x.UserName.Equals(userName));
    }

    public async Task<User> AddAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await dbContexts.Users.AddAsync(user);
        await dbContexts.SaveChangesAsync();
        User nuser = new User {
            Id = user.Id,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        };
        return nuser;
    }

    public async Task<bool> UserNameExistsAsync(string userName)
    {
        return await dbContexts.Users
                .AnyAsync(u => u.UserName == userName);
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        return await dbContexts.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
    }
}
