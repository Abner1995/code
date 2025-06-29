using ApiUser.Domain.Entities;
using ApiUser.Domain.Repositories;
using ApiUser.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Todo.Infrastructure.Core;

namespace ApiUser.Infrastructure.Repositories;

public class UsersRepository : Repository<User, long, UserDbContext>, IUsersRepository
{
    public UsersRepository(UserDbContext userDbContext) : base(userDbContext)
    {

    }

    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        return await DbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }
}
