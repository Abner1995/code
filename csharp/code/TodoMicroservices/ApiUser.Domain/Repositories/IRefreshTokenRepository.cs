using ApiUser.Domain.Entities;
using Todo.Infrastructure.Core;

namespace ApiUser.Domain.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken, long>
{

}
