using ApiUser.Domain.Entities;
using ApiUser.Domain.Repositories;
using ApiUser.Infrastructure.Persistence;
using Todo.Infrastructure.Core;

namespace ApiUser.Infrastructure.Repositories;

public class UsersRepository : Repository<User, long, UserDbContext>, IUsersRepository
{
    public UsersRepository(UserDbContext userDbContext) : base(userDbContext)
    {

    }
}
