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
}
