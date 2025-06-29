using ApiUser.Domain.Entities;
using Todo.Infrastructure.Core;

namespace ApiUser.Domain.Repositories;

public interface IUsersRepository : IRepository<User, long>
{
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken);
}
