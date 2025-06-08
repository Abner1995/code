using ApiUser.Domain.Entities;
using ApiUser.Domain.Repositories;
using ApiUser.Infrastructure.Persistence;
using Todo.Infrastructure.Core;

namespace ApiUser.Infrastructure.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken, long, UserDbContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(UserDbContext userDbContext) : base(userDbContext)
    {

    }
}
